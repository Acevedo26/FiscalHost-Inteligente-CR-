using CsvHelper;
using FiscalHost.Api.CR.Models.DTOs.Operations.Requests;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Enums;
using FiscalHost.Api.CR.Repositories;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace FiscalHost.Api.CR.Services;

public interface IImportacionMasivaService
{
	Task<object> ImportarAsync(IFormFile archivo, Guid usuarioId);
	Task<string> ObtenerReporteErroresCsvAsync(Guid importacionId);
	string GenerarPlantillaCsv();
}

public class ImportacionMasivaService(
	IImportacionMasivaRepository repository)
	: IImportacionMasivaService
{
	private static readonly string[] ColumnasObligatorias =
	[
		"FechaInicio",
		"FechaFin",
		"MontoBruto",
		"PlataformaOrigen",
		"ReferenciaPlataforma"
	];

	public async Task<object> ImportarAsync(
		IFormFile archivo,
		Guid usuarioId)
	{
		if (archivo == null || archivo.Length == 0)
			throw new Exception("El archivo está vacío o no fue enviado.");

		if (!archivo.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
			throw new Exception("Solo se permiten archivos CSV.");

		var importacion = new ImportacionMasiva
		{
			ImportacionId = Guid.NewGuid(),
			UsuarioId = usuarioId,
			TipoImportacion = "PLATAFORMA",
			ArchivoUrl = $"uploads/importaciones/{archivo.FileName}",
			NombreArchivoOriginal = archivo.FileName,
			PlantillaUtilizada = "PLANTILLA_HU_004",
			TamanioBytes = archivo.Length,
			Estado = EstadoImportacion.PROCESANDO,
			FechaInicioProcesamiento = DateTimeOffset.UtcNow
		};

		await repository.AddImportacionAsync(importacion);
		await repository.SaveChangesAsync();

		List<ImportacionReservaCsvRow> filas;

		try
		{
			using var stream = archivo.OpenReadStream();
			using var reader = new StreamReader(stream);
			using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

			if (!csv.Read())
				throw new Exception("El archivo no contiene encabezados.");

			csv.ReadHeader();

			var headers = csv.HeaderRecord ?? [];

			var faltantes = ColumnasObligatorias
				.Where(c => !headers.Contains(c))
				.ToList();

			if (faltantes.Any())
			{
				importacion.Estado = EstadoImportacion.RECHAZADO;
				importacion.DetalleErrores = JsonSerializer.Serialize(faltantes);
				importacion.ReporteErroresUrl =
					$"/api/importaciones/{importacion.ImportacionId}/errores";
				importacion.FechaFinProcesamiento = DateTimeOffset.UtcNow;

				await repository.SaveChangesAsync();

				throw new Exception(
					"La estructura del archivo es inválida. Columnas faltantes: " +
					string.Join(", ", faltantes));
			}

			filas = csv.GetRecords<ImportacionReservaCsvRow>().ToList();
		}
		catch (Exception ex)
		{
			if (importacion.Estado != EstadoImportacion.RECHAZADO)
			{
				importacion.Estado = EstadoImportacion.RECHAZADO;
				importacion.DetalleErrores = JsonSerializer.Serialize(
					new[] { ex.Message });
				importacion.ReporteErroresUrl =
					$"/api/importaciones/{importacion.ImportacionId}/errores";
				importacion.FechaFinProcesamiento = DateTimeOffset.UtcNow;

				await repository.SaveChangesAsync();
			}

			throw new Exception("El archivo CSV posee un formato inválido: " + ex.Message);
		}

		if (!filas.Any())
			throw new Exception("El archivo no contiene registros.");

		var errores = new List<string>();
		var reservasGuardar = new List<Reserva>();
		var referenciasArchivo = new HashSet<string>();

		var numeroFila = 1;

		foreach (var fila in filas)
		{
			numeroFila++;

			if (!DateTime.TryParse(fila.FechaInicio, out var fechaInicio))
			{
				errores.Add($"Fila {numeroFila}: FechaInicio inválida.");
				continue;
			}

			fechaInicio = DateTime.SpecifyKind(
				fechaInicio,
				DateTimeKind.Utc);

			if (!DateTime.TryParse(fila.FechaFin, out var fechaFin))
			{
				errores.Add($"Fila {numeroFila}: FechaFin inválida.");
				continue;
			}

			fechaFin = DateTime.SpecifyKind(
				fechaFin,
				DateTimeKind.Utc);

			if (!decimal.TryParse(
					fila.MontoBruto,
					NumberStyles.Number,
					CultureInfo.InvariantCulture,
					out var montoBruto))
			{
				errores.Add($"Fila {numeroFila}: MontoBruto inválido.");
				continue;
			}

			if (montoBruto <= 0)
			{
				errores.Add($"Fila {numeroFila}: MontoBruto debe ser mayor que cero.");
				continue;
			}

			if (fechaInicio > DateTime.UtcNow)
			{
				errores.Add($"Fila {numeroFila}: FechaInicio no puede ser futura.");
				continue;
			}

			if (fechaFin < fechaInicio)
			{
				errores.Add($"Fila {numeroFila}: FechaFin no puede ser menor que FechaInicio.");
				continue;
			}

			if (!TryParsePlataforma(fila.PlataformaOrigen, out var plataforma))
			{
				errores.Add($"Fila {numeroFila}: PlataformaOrigen inválida.");
				continue;
			}

			var referencia = fila.ReferenciaPlataforma?.Trim();

			if (string.IsNullOrWhiteSpace(referencia))
			{
				errores.Add($"Fila {numeroFila}: ReferenciaPlataforma es obligatoria.");
				continue;
			}

			if (!referenciasArchivo.Add(referencia))
			{
				errores.Add($"Fila {numeroFila}: ReferenciaPlataforma duplicada en el archivo.");
				continue;
			}

			if (await repository.ExisteReferenciaAsync(usuarioId, referencia))
			{
				errores.Add($"Fila {numeroFila}: La reserva {referencia} ya existe en el sistema.");
				continue;
			}

			var diasEstancia = (fechaFin.Date - fechaInicio.Date).Days;

			var iva = diasEstancia < 30
				? montoBruto * 0.13m
				: 0m;

			var retencion = EsPlataformaExtranjera(plataforma)
				? montoBruto * 0.15m
				: 0m;

			var baseRenta = montoBruto * 0.85m;
			var impuestoRenta = baseRenta * 0.15m;
			var impuestoNeto = impuestoRenta - retencion;

			var saldoFavor = impuestoNeto < 0
				? Math.Abs(impuestoNeto)
				: 0m;

			if (impuestoNeto < 0)
				impuestoNeto = 0m;

			var ingresoNeto = montoBruto - iva - impuestoNeto;

			var clasificacion = diasEstancia >= 30
				? ClasificacionFiscal.EXENTO
				: retencion > 0
					? ClasificacionFiscal.GRAVADO_CON_RETENCION
					: ClasificacionFiscal.GRAVADO;

			reservasGuardar.Add(new Reserva
			{
				ReservaId = Guid.NewGuid(),
				UsuarioId = usuarioId,
				ImportacionId = importacion.ImportacionId,

				FechaInicio = fechaInicio,
				FechaFin = fechaFin,

				MontoBruto = montoBruto,
				Moneda = TipoMoneda.CRC,
				TipoCambio = 1,
				MontoColones = montoBruto,

				ClasificacionFiscal = clasificacion,

				MontoGravado = diasEstancia < 30 ? montoBruto : 0m,
				MontoExento = diasEstancia >= 30 ? montoBruto : 0m,
				MontoIvaCalculado = iva,
				RetencionExtranjera = retencion,

				PlataformaOrigen = plataforma,
				FuenteRegistro = FuenteRegistro.IMPORTACION_CSV,
				ReferenciaPlataforma = referencia,

				FueReclasificada = false,
				PeriodoFiscalAnio = (short)fechaInicio.Year,
				PeriodoFiscalMes = (short)fechaInicio.Month,
				Estado = "EN_REVISION",

				Metadata = JsonSerializer.Serialize(new
				{
					diasEstancia,
					baseRenta,
					impuestoRenta,
					impuestoNeto,
					saldoFavor,
					ingresoNeto
				})
			});
		}

		if (reservasGuardar.Any())
			await repository.AddReservasAsync(reservasGuardar);

		importacion.TotalRegistros = filas.Count;
		importacion.RegistrosExitosos = reservasGuardar.Count;
		importacion.RegistrosConError = errores.Count;
		importacion.DetalleErrores = JsonSerializer.Serialize(errores);
		importacion.FechaFinProcesamiento = DateTimeOffset.UtcNow;

		importacion.Estado = errores.Count == 0
			? EstadoImportacion.COMPLETADO
			: reservasGuardar.Count > 0
				? EstadoImportacion.COMPLETADO_PARCIAL
				: EstadoImportacion.RECHAZADO;

		if (errores.Any())
		{
			importacion.ReporteErroresUrl =
				$"/api/importaciones/{importacion.ImportacionId}/errores";
		}

		await repository.SaveChangesAsync();

		return new
		{
			Success = true,
			ImportacionId = importacion.ImportacionId,
			Estado = importacion.Estado.ToString(),
			TotalRegistros = importacion.TotalRegistros,
			RegistrosExitosos = importacion.RegistrosExitosos,
			RegistrosConError = importacion.RegistrosConError,
			ReporteErroresUrl = importacion.ReporteErroresUrl,
			Errores = errores
		};
	}

	public async Task<string> ObtenerReporteErroresCsvAsync(Guid importacionId)
	{
		var importacion = await repository.GetImportacionAsync(importacionId);

		if (importacion is null)
			throw new Exception("No se encontró la importación.");

		var errores = JsonSerializer.Deserialize<List<string>>(
			importacion.DetalleErrores) ?? [];

		var sb = new StringBuilder();
		sb.AppendLine("Error");

		foreach (var error in errores)
			sb.AppendLine($"\"{error.Replace("\"", "\"\"")}\"");

		return sb.ToString();
	}

	public string GenerarPlantillaCsv()
	{
		return
			"FechaInicio,FechaFin,MontoBruto,PlataformaOrigen,ReferenciaPlataforma\n" +
			"2026-06-01,2026-06-05,250000,AIRBNB,AIR-001\n";
	}

	private static bool EsPlataformaExtranjera(PlataformaOrigen plataforma)
	{
		return plataforma is PlataformaOrigen.AIRBNB
			or PlataformaOrigen.BOOKING
			or PlataformaOrigen.VRBO;
	}

	private static bool TryParsePlataforma(
		string? valor,
		out PlataformaOrigen plataforma)
	{
		plataforma = PlataformaOrigen.OTRA;

		if (string.IsNullOrWhiteSpace(valor))
			return false;

		var normalizado = valor.Trim().ToUpperInvariant();

		if (normalizado == "DIRECTO")
			normalizado = "DIRECTA";

		return Enum.TryParse(normalizado, true, out plataforma);
	}
}