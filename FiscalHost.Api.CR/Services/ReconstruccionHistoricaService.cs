using CsvHelper;
using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums.TaxIntelligence;
using FiscalHost.Api.CR.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace FiscalHost.Api.CR.Services;

public class ReconstruccionHistoricaService(
    IReconstruccionHistoricaRepository reconstruccionRepo,
    IPeriodoFiscalRepository periodoRepo,
    IObligacionTributariaRepository obligacionRepo,
    AppDbContext dbContext) : IReconstruccionHistoricaService
{
    private static readonly string[] ColumnasRequeridas =
    [
        "Anio", "Mes", "TipoFormulario", "IngresosBrutos", "IngresosGravados",
        "IngresosExentos", "GastosDeducibles", "Retenciones", "CreditoFiscal"
    ];

    public async Task<ReconstruccionHistoricaResponse> ReconstruirAsync(ReconstruccionHistoricaRequest request)
    {
        if (request.Archivo == null || request.Archivo.Length == 0)
            throw new Exception("El archivo está vacío o no fue enviado.");

        if (!request.Archivo.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            throw new Exception("Solo se permiten archivos CSV.");

        var reconstruccion = new ReconstruccionHistorica
        {
            ReconstruccionId = Guid.NewGuid(),
            UsuarioId = request.UsuarioId,
            NombreArchivoOriginal = request.Archivo.FileName,
            TamanioBytes = request.Archivo.Length,
            Estado = EstadoImportacion.PROCESANDO,
            FechaInicioProcesamiento = DateTimeOffset.UtcNow
        };

        await reconstruccionRepo.AddAsync(reconstruccion);
        await reconstruccionRepo.SaveChangesAsync();

        List<ReconstruccionHistoricaCsvRow> filas;

        try
        {
            using var stream = request.Archivo.OpenReadStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            if (!csv.Read())
                throw new Exception("El archivo no contiene encabezados.");

            csv.ReadHeader();
            var headers = csv.HeaderRecord ?? [];

            var faltantes = ColumnasRequeridas
                .Where(c => !headers.Contains(c, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (faltantes.Any())
            {
                reconstruccion.Estado = EstadoImportacion.RECHAZADO;
                reconstruccion.DetalleErrores = JsonSerializer.Serialize(faltantes);
                reconstruccion.ReporteErroresUrl = $"/api/reconstruccion/{reconstruccion.ReconstruccionId}/errores";
                reconstruccion.FechaFinProcesamiento = DateTimeOffset.UtcNow;
                await reconstruccionRepo.SaveChangesAsync();

                throw new Exception("La estructura del archivo es inválida. Columnas faltantes: " + string.Join(", ", faltantes));
            }

            filas = csv.GetRecords<ReconstruccionHistoricaCsvRow>().ToList();
        }
        catch (Exception ex)
        {
            if (reconstruccion.Estado != EstadoImportacion.RECHAZADO)
            {
                reconstruccion.Estado = EstadoImportacion.RECHAZADO;
                reconstruccion.DetalleErrores = JsonSerializer.Serialize(new[] { ex.Message });
                reconstruccion.ReporteErroresUrl = $"/api/reconstruccion/{reconstruccion.ReconstruccionId}/errores";
                reconstruccion.FechaFinProcesamiento = DateTimeOffset.UtcNow;
                await reconstruccionRepo.SaveChangesAsync();
            }

            throw new Exception("El archivo CSV posee un formato inválido: " + ex.Message);
        }

        if (!filas.Any())
            throw new Exception("El archivo no contiene registros.");

        var errores = new List<string>();
        var resultados = new List<ReconstruccionHistoricaFilaResultado>();
        var calculosGuardar = new List<CalculoFiscal>();
        var obligacionesGuardar = new List<ObligacionTributaria>();

        var numeroFila = 1;

        foreach (var fila in filas)
        {
            numeroFila++;

            var resultadoFila = new ReconstruccionHistoricaFilaResultado
            {
                Anio = fila.Anio,
                Mes = fila.Mes,
                TipoFormulario = ParseTipoFormulario(fila.TipoFormulario)
            };

            if (!Enum.TryParse<TipoFormulario>(fila.TipoFormulario, true, out var tipoFormulario))
            {
                resultadoFila.EsValida = false;
                resultadoFila.Error = $"Fila {numeroFila}: TipoFormulario '{fila.TipoFormulario}' inválido. Valores permitidos: D104, D125, D150, D116, D176.";
                errores.Add(resultadoFila.Error);
                resultados.Add(resultadoFila);
                continue;
            }
            resultadoFila.TipoFormulario = tipoFormulario;

            if (fila.Anio < 2020 || fila.Anio > 2026)
            {
                resultadoFila.EsValida = false;
                resultadoFila.Error = $"Fila {numeroFila}: Año {fila.Anio} fuera de rango (2020-2026).";
                errores.Add(resultadoFila.Error);
                resultados.Add(resultadoFila);
                continue;
            }

            if (fila.Mes < 1 || fila.Mes > 12)
            {
                resultadoFila.EsValida = false;
                resultadoFila.Error = $"Fila {numeroFila}: Mes {fila.Mes} inválido (1-12).";
                errores.Add(resultadoFila.Error);
                resultados.Add(resultadoFila);
                continue;
            }

            var periodo = await periodoRepo.GetByAnioMesTipoAsync(fila.Anio, fila.Mes, tipoFormulario);
            if (periodo is null)
            {
                resultadoFila.EsValida = false;
                resultadoFila.Error = $"Fila {numeroFila}: No existe PeriodoFiscal para {fila.Mes:00}/{fila.Anio} - {tipoFormulario}.";
                errores.Add(resultadoFila.Error);
                resultados.Add(resultadoFila);
                continue;
            }

            decimal ivaCalculado = 0;
            decimal rentaCapitalCalculada = 0;
            decimal multaInscripcionTardia = 0;

            if (tipoFormulario == TipoFormulario.D104 || tipoFormulario == TipoFormulario.D176)
            {
                ivaCalculado = Math.Round(fila.IngresosGravados * (periodo.TarifaIva / 100m) - fila.CreditoFiscal, 2);
                if (ivaCalculado < 0) ivaCalculado = 0;
            }

            if (tipoFormulario == TipoFormulario.D125 || tipoFormulario == TipoFormulario.D176)
            {
                var baseImponible = Math.Max(fila.IngresosBrutos - fila.GastosDeducibles - periodo.DeduccionPlanaCapital, 0m);
                rentaCapitalCalculada = Math.Round(baseImponible * (periodo.TarifaRentaCapital / 100m), 2);
            }

            if (tipoFormulario == TipoFormulario.D176 && periodo.SalarioBaseVigente.HasValue)
            {
                multaInscripcionTardia = Math.Round(0.5m * periodo.SalarioBaseVigente.Value, 2);
            }

            var montoTotalObligacion = ivaCalculado + rentaCapitalCalculada + multaInscripcionTardia;

            var calculoId = Guid.NewGuid();
            var calculo = new CalculoFiscal
            {
                CalculoId = calculoId,
                UsuarioId = request.UsuarioId,
                PeriodoId = periodo.PeriodoId,
                TipoFormulario = tipoFormulario,
                Estado = EstadoDeclaracion.CALCULADO,
                TotalIngresosBrutos = fila.IngresosBrutos,
                TotalIngresosGravados = fila.IngresosGravados,
                TotalIngresosExentos = fila.IngresosExentos,
                DebitoFiscal = Math.Round(fila.IngresosGravados * (periodo.TarifaIva / 100m), 2),
                CreditoFiscal = fila.CreditoFiscal,
                IvaNeto = ivaCalculado,
                SaldoFavorAnterior = 0,
                SaldoFavorResultante = ivaCalculado < 0 ? Math.Abs(ivaCalculado) : 0,
                RentaBruta = fila.IngresosBrutos,
                DeduccionAplicada = periodo.DeduccionPlanaCapital,
                RentaNeta = Math.Max(fila.IngresosBrutos - fila.GastosDeducibles - periodo.DeduccionPlanaCapital, 0m),
                ImpuestoRenta = rentaCapitalCalculada,
                RetencionesAcreditadas = fila.Retenciones,
                MontoTotalAPagar = montoTotalObligacion,
                BorradorGenerado = false,
                DetalleCalculo = JsonSerializer.Serialize(new
                {
                    filaOriginal = new
                    {
                        fila.Anio,
                        fila.Mes,
                        fila.TipoFormulario,
                        fila.IngresosBrutos,
                        fila.IngresosGravados,
                        fila.IngresosExentos,
                        fila.GastosDeducibles,
                        fila.Retenciones,
                        fila.CreditoFiscal
                    },
                    tarifaIva = periodo.TarifaIva,
                    tarifaRentaCapital = periodo.TarifaRentaCapital,
                    deduccionPlanaCapital = periodo.DeduccionPlanaCapital,
                    salarioBaseVigente = periodo.SalarioBaseVigente,
                    ivaCalculado,
                    rentaCapitalCalculada,
                    multaInscripcionTardia,
                    montoTotalObligacion,
                    calculadoEn = DateTimeOffset.UtcNow
                }),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            var obligacionId = Guid.NewGuid();
            var obligacion = new ObligacionTributaria
            {
                ObligacionId = obligacionId,
                UsuarioId = request.UsuarioId,
                CalculoId = calculoId,
                PeriodoId = periodo.PeriodoId,
                TipoFormulario = tipoFormulario,
                Descripcion = $"Reconstrucción histórica {tipoFormulario} - {fila.Mes:00}/{fila.Anio}",
                MontoCapital = montoTotalObligacion,
                MontoMulta = multaInscripcionTardia,
                MontoInteresesAcumulados = 0,
                MontoTotalActualizado = montoTotalObligacion,
                FechaVencimiento = new DateOnly(fila.Anio, fila.Mes, DateTime.DaysInMonth(fila.Anio, fila.Mes)),
                Estado = EstadoObligacion.VIGENTE,
                TasaInteresAplicada = periodo.TasaInteresMoraAnual,
                HistorialIntereses = "{}",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            calculosGuardar.Add(calculo);
            obligacionesGuardar.Add(obligacion);

            resultadoFila.EsValida = true;
            resultadoFila.CalculoId = calculoId;
            resultadoFila.ObligacionId = obligacionId;
            resultadoFila.IvaCalculado = ivaCalculado;
            resultadoFila.RentaCapitalCalculada = rentaCapitalCalculada;
            resultadoFila.MultaInscripcionTardia = multaInscripcionTardia;
            resultadoFila.MontoTotalObligacion = montoTotalObligacion;
            resultados.Add(resultadoFila);
        }

        if (calculosGuardar.Any())
        {
            await dbContext.CalculosFiscales.AddRangeAsync(calculosGuardar);
            await dbContext.ObligacionesTributarias.AddRangeAsync(obligacionesGuardar);
        }

        reconstruccion.TotalRegistros = filas.Count;
        reconstruccion.RegistrosExitosos = resultados.Count(r => r.EsValida);
        reconstruccion.RegistrosConError = errores.Count;
        reconstruccion.DetalleErrores = JsonSerializer.Serialize(errores);
        reconstruccion.FechaFinProcesamiento = DateTimeOffset.UtcNow;

        reconstruccion.Estado = errores.Count == 0
            ? EstadoImportacion.COMPLETADO
            : reconstruccion.RegistrosExitosos > 0
                ? EstadoImportacion.COMPLETADO_PARCIAL
                : EstadoImportacion.RECHAZADO;

        if (errores.Any())
        {
            reconstruccion.ReporteErroresUrl = $"/api/reconstruccion/{reconstruccion.ReconstruccionId}/errores";
        }

        await reconstruccionRepo.UpdateAsync(reconstruccion);
        await reconstruccionRepo.SaveChangesAsync();

        return new ReconstruccionHistoricaResponse
        {
            ReconstruccionId = reconstruccion.ReconstruccionId,
            UsuarioId = reconstruccion.UsuarioId,
            TotalFilas = reconstruccion.TotalRegistros,
            FilasExitosas = reconstruccion.RegistrosExitosos,
            FilasConError = reconstruccion.RegistrosConError,
            Estado = reconstruccion.Estado.ToString(),
            ReporteErroresUrl = reconstruccion.ReporteErroresUrl,
            Resultados = resultados
        };
    }

    public async Task<DetectarPeriodosFaltantesResponse> DetectarPeriodosFaltantesAsync(DetectarPeriodosFaltantesRequest request)
    {
        var periodosExistentes = await periodoRepo.GetByAnioRangoTipoAsync(request.AnioInicio, request.AnioFin, request.TipoFormulario);
        var existentesSet = new HashSet<(short Anio, short Mes)>(periodosExistentes.Select(p => (p.Anio, p.Mes)));

        var periodosFaltantes = new List<PeriodoFaltanteDto>();

        for (short anio = request.AnioInicio; anio <= request.AnioFin; anio++)
        {
            for (short mes = 1; mes <= 12; mes++)
            {
                if (!existentesSet.Contains((anio, mes)))
                {
                    periodosFaltantes.Add(new PeriodoFaltanteDto
                    {
                        Anio = anio,
                        Mes = mes,
                        TipoFormulario = request.TipoFormulario,
                        Mensaje = $"Período {mes:00}/{anio} no tiene configuración fiscal para {request.TipoFormulario}"
                    });
                }
            }
        }

        var totalEsperados = (request.AnioFin - request.AnioInicio + 1) * 12;

        return new DetectarPeriodosFaltantesResponse
        {
            UsuarioId = request.UsuarioId,
            AnioInicio = request.AnioInicio,
            AnioFin = request.AnioFin,
            TipoFormulario = request.TipoFormulario,
            TotalPeriodosEsperados = totalEsperados,
            TotalPeriodosExistentes = periodosExistentes.Count,
            TotalPeriodosFaltantes = periodosFaltantes.Count,
            PeriodosFaltantes = periodosFaltantes
        };
    }

    public async Task<string> ObtenerReporteErroresAsync(Guid reconstruccionId)
    {
        var reconstruccion = await reconstruccionRepo.GetByIdAsync(reconstruccionId);

        if (reconstruccion is null)
            throw new Exception("No se encontró la reconstrucción histórica.");

        var errores = JsonSerializer.Deserialize<List<string>>(reconstruccion.DetalleErrores) ?? [];

        var sb = new StringBuilder();
        sb.AppendLine("Error");

        foreach (var error in errores)
            sb.AppendLine($"\"{error.Replace("\"", "\"\"")}\"");

        return sb.ToString();
    }

    private static TipoFormulario ParseTipoFormulario(string valor)
    {
        if (Enum.TryParse<TipoFormulario>(valor, true, out var resultado))
            return resultado;
        return TipoFormulario.D176;
    }
}