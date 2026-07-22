using System.Globalization;
using System.Text;
using System.Text.Json;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums.Operations;
using FiscalHost.Api.CR.Models.Enums.TaxIntelligence;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public interface IReconstruccionBaseImponibleService
{
    Task<ReconstruccionBaseImponibleResponse> ReconstruirAsync(ReconstruccionBaseImponibleRequest request);
    ValidacionHistoricoResponse ValidarArchivoHistorico(IFormFile archivo);
    string GenerarPlantillaHistoricosCsv();
}

public class ReconstruccionBaseImponibleService(
    IReconstruccionBaseImponibleRepository repository)
    : IReconstruccionBaseImponibleService
{
    private static readonly string[] ColumnasHistoricas =
    [
        "FechaInicio",
        "FechaFin",
        "MontoBruto",
        "MontoGravado",
        "MontoExento",
        "RetencionExtranjera"
    ];

    public async Task<ReconstruccionBaseImponibleResponse> ReconstruirAsync(
        ReconstruccionBaseImponibleRequest request)
    {
        if (request.UsuarioId == Guid.Empty)
            return Error(request, "El usuario es obligatorio.");

        if (request.AnioFiscal < 2019 || request.AnioFiscal > DateTime.UtcNow.Year)
            return Error(request, "El año fiscal solicitado no es válido.");

        var reservas = await repository.GetReservasPorAnioAsync(request.UsuarioId, request.AnioFiscal);
        var periodos = await repository.GetPeriodosPorAnioAsync(request.AnioFiscal);

        var basesMensuales = Enumerable.Range(1, 12)
            .Select(mes => ReconstruirMes(mes, reservas, periodos))
            .ToList();

        var mesesSinDatos = basesMensuales
            .Where(m => !m.TieneDatos)
            .Select(m => m.Mes)
            .ToList();

        var mesesSinNormativa = basesMensuales
            .Where(m => m.TieneDatos && !m.TieneNormativaHistorica)
            .Select(m => m.Mes)
            .ToList();

        if ((mesesSinDatos.Any() || mesesSinNormativa.Any()) &&
            !request.ContinuarConDatosIncompletos)
        {
            return new ReconstruccionBaseImponibleResponse
            {
                Success = false,
                Mensaje = "Se detectaron datos historicos incompletos. Revise los meses informados antes de continuar.",
                UsuarioId = request.UsuarioId,
                AnioFiscal = request.AnioFiscal,
                MesesSinDatos = mesesSinDatos,
                MesesSinNormativa = mesesSinNormativa,
                BasesMensuales = basesMensuales,
                Consolidado = Consolidar(basesMensuales)
            };
        }

        var calculos = ConstruirCalculos(request.UsuarioId, basesMensuales, periodos);

        if (calculos.Any())
        {
            await repository.AddCalculosAsync(calculos);
            await repository.SaveChangesAsync();
        }

        return new ReconstruccionBaseImponibleResponse
        {
            Success = true,
            Mensaje = "Reconstruccion de bases imponibles completada.",
            UsuarioId = request.UsuarioId,
            AnioFiscal = request.AnioFiscal,
            MesesSinDatos = mesesSinDatos,
            MesesSinNormativa = mesesSinNormativa,
            BasesMensuales = basesMensuales,
            Consolidado = Consolidar(basesMensuales)
        };
    }

    public ValidacionHistoricoResponse ValidarArchivoHistorico(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
        {
            return new ValidacionHistoricoResponse
            {
                Success = false,
                Mensaje = "El archivo historico esta vacio o no fue enviado."
            };
        }

        if (!archivo.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return new ValidacionHistoricoResponse
            {
                Success = false,
                Mensaje = "Formato incompatible. Solo se permiten archivos CSV."
            };
        }

        using var stream = archivo.OpenReadStream();
        using var reader = new StreamReader(stream);
        var headerLine = reader.ReadLine();

        if (string.IsNullOrWhiteSpace(headerLine))
        {
            return new ValidacionHistoricoResponse
            {
                Success = false,
                Mensaje = "El archivo no contiene encabezados."
            };
        }

        var headers = headerLine.Split(',')
            .Select(h => h.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var faltantes = ColumnasHistoricas
            .Where(c => !headers.Contains(c))
            .ToList();

        return new ValidacionHistoricoResponse
        {
            Success = !faltantes.Any(),
            Mensaje = faltantes.Any()
                ? "La estructura del archivo historico es invalida."
                : "La estructura del archivo historico es valida.",
            ColumnasFaltantes = faltantes
        };
    }

    public string GenerarPlantillaHistoricosCsv()
    {
        return string.Join(",", ColumnasHistoricas) + Environment.NewLine +
               "2025-01-01,2025-01-05,250000,250000,0,0" + Environment.NewLine;
    }

    private static ReconstruccionMensualDto ReconstruirMes(
        int mes,
        List<Reserva> reservas,
        List<PeriodoFiscal> periodos)
    {
        var reservasMes = reservas
            .Where(r => r.PeriodoFiscalMes == mes)
            .ToList();

        var periodo = periodos.FirstOrDefault(p =>
            p.Mes == mes && p.TipoFormulario == TipoFormulario.D104)
            ?? periodos.FirstOrDefault(p => p.Mes == mes);

        var tarifaIva = NormalizarPorcentaje(periodo?.TarifaIva ?? 0.13m);
        var tarifaRenta = NormalizarPorcentaje(periodo?.TarifaRentaCapital ?? 0.15m);
        var deduccion = NormalizarDeduccion(periodo?.DeduccionPlanaCapital ?? 0.85m);

        var ingresosBrutos = reservasMes.Sum(MontoEnColones);
        var ingresosGravados = reservasMes.Sum(r =>
            r.MontoGravado > 0 ? r.MontoGravado :
            r.ClasificacionFiscal == ClasificacionFiscal.GRAVADO ||
            r.ClasificacionFiscal == ClasificacionFiscal.GRAVADO_CON_RETENCION
                ? MontoEnColones(r)
                : 0);

        var ingresosExentos = reservasMes.Sum(r =>
            r.MontoExento > 0 ? r.MontoExento :
            r.ClasificacionFiscal == ClasificacionFiscal.EXENTO
                ? MontoEnColones(r)
                : 0);

        var retenciones = reservasMes.Sum(r => r.RetencionExtranjera);
        var debitoFiscal = ingresosGravados * tarifaIva;
        var baseRenta = ingresosBrutos * deduccion;
        var impuestoRenta = baseRenta * tarifaRenta;
        var totalAPagar = Math.Max(0, debitoFiscal + impuestoRenta - retenciones);

        return new ReconstruccionMensualDto
        {
            Mes = mes,
            TieneDatos = reservasMes.Any(),
            TieneNormativaHistorica = periodo is not null,
            TarifaIvaAplicada = tarifaIva,
            TarifaRentaAplicada = tarifaRenta,
            DeduccionAplicada = deduccion,
            IngresosBrutos = ingresosBrutos,
            IngresosGravados = ingresosGravados,
            IngresosExentos = ingresosExentos,
            DebitoFiscal = debitoFiscal,
            RetencionesAcreditadas = retenciones,
            BaseImponibleRenta = baseRenta,
            ImpuestoRenta = impuestoRenta,
            TotalAPagar = totalAPagar
        };
    }

    private static List<CalculoFiscal> ConstruirCalculos(
        Guid usuarioId,
        List<ReconstruccionMensualDto> basesMensuales,
        List<PeriodoFiscal> periodos)
    {
        return basesMensuales
            .Where(b => b.TieneDatos && b.TieneNormativaHistorica)
            .Select(b =>
            {
                var periodo = periodos.First(p =>
                    p.Mes == b.Mes && (p.TipoFormulario == TipoFormulario.D104 ||
                    periodos.All(x => x.Mes != b.Mes || x.TipoFormulario != TipoFormulario.D104)));

                return new CalculoFiscal
                {
                    CalculoId = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    PeriodoId = periodo.PeriodoId,
                    TipoFormulario = periodo.TipoFormulario,
                    RegimenAplicado = RegimenTributario.CAPITAL_INMOBILIARIO,
                    Estado = EstadoDeclaracion.BORRADOR,
                    TotalIngresosBrutos = b.IngresosBrutos,
                    TotalIngresosGravados = b.IngresosGravados,
                    TotalIngresosExentos = b.IngresosExentos,
                    DebitoFiscal = b.DebitoFiscal,
                    CreditoFiscal = 0,
                    IvaNeto = b.DebitoFiscal,
                    SaldoFavorAnterior = 0,
                    SaldoFavorResultante = 0,
                    RentaBruta = b.IngresosBrutos,
                    DeduccionAplicada = b.DeduccionAplicada,
                    RentaNeta = b.BaseImponibleRenta,
                    ImpuestoRenta = b.ImpuestoRenta,
                    RetencionesAcreditadas = b.RetencionesAcreditadas,
                    MontoTotalAPagar = b.TotalAPagar,
                    DetalleCalculo = JsonSerializer.Serialize(b),
                    BorradorGenerado = false
                };
            })
            .ToList();
    }

    private static ReconstruccionConsolidadoDto Consolidar(List<ReconstruccionMensualDto> bases) => new()
    {
        TotalIngresosBrutos = bases.Sum(b => b.IngresosBrutos),
        TotalIngresosGravados = bases.Sum(b => b.IngresosGravados),
        TotalIngresosExentos = bases.Sum(b => b.IngresosExentos),
        TotalDebitoFiscal = bases.Sum(b => b.DebitoFiscal),
        TotalRetenciones = bases.Sum(b => b.RetencionesAcreditadas),
        TotalBaseImponibleRenta = bases.Sum(b => b.BaseImponibleRenta),
        TotalImpuestoRenta = bases.Sum(b => b.ImpuestoRenta),
        TotalAPagar = bases.Sum(b => b.TotalAPagar)
    };

    private static decimal MontoEnColones(Reserva reserva)
    {
        return reserva.MontoColones > 0 ? reserva.MontoColones : reserva.MontoBruto;
    }

    private static decimal NormalizarPorcentaje(decimal valor)
    {
        return valor > 1 ? valor / 100 : valor;
    }

    private static decimal NormalizarDeduccion(decimal valor)
    {
        var normalizado = NormalizarPorcentaje(valor);
        return normalizado <= 0.15m ? 1 - normalizado : normalizado;
    }

    private static ReconstruccionBaseImponibleResponse Error(
        ReconstruccionBaseImponibleRequest request,
        string mensaje) => new()
    {
        Success = false,
        Mensaje = mensaje,
        UsuarioId = request.UsuarioId,
        AnioFiscal = request.AnioFiscal
    };
}
