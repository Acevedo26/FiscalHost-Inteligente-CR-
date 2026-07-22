using System.Text.Json;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;
using FiscalHost.Api.CR.Models.Entities.Audit;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public interface ICalculoRentaCapitalService
{
	Task<(bool success, string? error, RentaCapitalResponse? data)> CalcularAsync(
		CalcularRentaCapitalRequest request);

	Task<(bool success, string? error, SimulacionRegimenResponse? data)> SimularAsync(
		SimularRegimenRequest request);

	Task<(bool success, string? error, string? regimenActual)> CambiarRegimenAsync(
		CambiarRegimenTributarioRequest request);
}

public class CalculoRentaCapitalService(
	ICalculoRentaCapitalRepository repository)
	: ICalculoRentaCapitalService
{
	public async Task<(bool success, string? error, RentaCapitalResponse? data)> CalcularAsync(
		CalcularRentaCapitalRequest request)
	{
		var perfil = await repository.GetPerfilTributarioAsync(request.UsuarioId);
		if (perfil is null)
			return (false, "No existe un perfil tributario registrado para este usuario.", null);

		var periodo = await repository.GetPeriodoFiscalAsync(request.Anio, request.Mes, TipoFormulario.D125);
		if (periodo is null)
			return (false,
				$"No existe una configuracion de periodo fiscal para {request.Mes:00}/{request.Anio}. Debe registrarse primero.",
				null);

		var reservas = await repository.GetReservasDelPeriodoAsync(request.UsuarioId, request.Anio, request.Mes);
		if (reservas.Count == 0)
			return (false, "No hay ingresos registrados para el periodo seleccionado.", null);

		var rentaBruta = reservas.Sum(r => r.MontoColones);

		decimal deduccion;
		if (perfil.RegimenTributario == RegimenTributario.UTILIDADES)
		{
			var gastosValidos = await ObtenerGastosDeduciblesValidosAsync(request.UsuarioId, request.Anio, request.Mes);
			if (gastosValidos.Count == 0)
				return (false, "Debe adjuntar los comprobantes de gastos para justificar el cambio de regimen.", null);

			deduccion = gastosValidos.Sum(g => g.MontoNeto ?? g.MontoColones);
		}
		else
		{
			// periodo.DeduccionPlanaCapital se guarda como porcentaje entero (ej. 15.00 = 15%),
			// no como fraccion decimal, por eso se divide entre 100.
			deduccion = rentaBruta * (periodo.DeduccionPlanaCapital / 100m);
		}

		var rentaNeta = Math.Max(rentaBruta - deduccion, 0m);
		var impuestoRenta = rentaNeta * (periodo.TarifaRentaCapital / 100m);

		var calculoExistente = await repository.GetCalculoExistenteAsync(request.UsuarioId, periodo.PeriodoId, TipoFormulario.D125);
		var calculo = calculoExistente
					  ?? new CalculoFiscal { CalculoId = Guid.NewGuid(), UsuarioId = request.UsuarioId, PeriodoId = periodo.PeriodoId, TipoFormulario = TipoFormulario.D125 };

		if (calculoExistente is null)
			await repository.AddCalculoAsync(calculo);

		calculo.RegimenAplicado = perfil.RegimenTributario;
		calculo.Estado = EstadoDeclaracion.CALCULADO;

		// Campos de IVA no aplican a una declaracion de renta (D125); se
		// dejan en 0 porque calculo_fiscal es una tabla compartida con HU-008.
		calculo.TotalIngresosGravados = 0;
		calculo.TotalIngresosExentos = 0;
		calculo.DebitoFiscal = 0;
		calculo.CreditoFiscal = 0;
		calculo.IvaNeto = 0;
		calculo.SaldoFavorAnterior = 0;
		calculo.SaldoFavorResultante = 0;

		calculo.TotalIngresosBrutos = rentaBruta;
		calculo.RentaBruta = rentaBruta;
		calculo.DeduccionAplicada = deduccion;
		calculo.RentaNeta = rentaNeta;
		calculo.ImpuestoRenta = impuestoRenta;
		calculo.RetencionesAcreditadas ??= 0m;
		calculo.MontoTotalAPagar = impuestoRenta - calculo.RetencionesAcreditadas.Value;
		calculo.BorradorGenerado = true;
		calculo.FechaGeneracionBorrador = DateTimeOffset.UtcNow;
		calculo.UpdatedAt = DateTimeOffset.UtcNow;
		calculo.DetalleCalculo = JsonSerializer.Serialize(new
		{
			regimen = perfil.RegimenTributario.ToString(),
			rentaBruta,
			deduccion,
			rentaNeta,
			tarifaRentaCapital = periodo.TarifaRentaCapital,
			cantidadReservas = reservas.Count
		});

		await repository.SaveChangesAsync();

		return (true, null, MapToResponse(calculo, periodo.Anio, periodo.Mes));
	}

	public async Task<(bool success, string? error, SimulacionRegimenResponse? data)> SimularAsync(
		SimularRegimenRequest request)
	{
		var periodo = await repository.GetPeriodoFiscalAsync(request.Anio, request.Mes, TipoFormulario.D125);
		if (periodo is null)
			return (false,
				$"No existe una configuracion de periodo fiscal para {request.Mes:00}/{request.Anio}. Debe registrarse primero.",
				null);

		var reservas = await repository.GetReservasDelPeriodoAsync(request.UsuarioId, request.Anio, request.Mes);
		if (reservas.Count == 0)
			return (false, "No hay ingresos registrados para el periodo seleccionado.", null);

		var rentaBruta = reservas.Sum(r => r.MontoColones);

		var deduccionCapital = rentaBruta * (periodo.DeduccionPlanaCapital / 100m);
		var rentaNetaCapital = Math.Max(rentaBruta - deduccionCapital, 0m);
		var impuestoCapital = rentaNetaCapital * (periodo.TarifaRentaCapital / 100m);

		var gastosValidos = await ObtenerGastosDeduciblesValidosAsync(request.UsuarioId, request.Anio, request.Mes);
		var tieneComprobantesValidos = gastosValidos.Count > 0;
		var deduccionUtilidades = gastosValidos.Sum(g => g.MontoNeto ?? g.MontoColones);
		var rentaNetaUtilidades = Math.Max(rentaBruta - deduccionUtilidades, 0m);
		var impuestoUtilidades = rentaNetaUtilidades * (periodo.TarifaRentaCapital / 100m);

		string recomendado;
		decimal ahorro;
		string justificacion;

		if (!tieneComprobantesValidos)
		{
			recomendado = RegimenTributario.CAPITAL_INMOBILIARIO.ToString();
			ahorro = 0m;
			justificacion = "No cuenta con comprobantes de gastos validados para calcular el Regimen de Utilidades; se recomienda mantener el Regimen de Capital Inmobiliario.";
		}
		else if (impuestoUtilidades < impuestoCapital)
		{
			recomendado = RegimenTributario.UTILIDADES.ToString();
			ahorro = impuestoCapital - impuestoUtilidades;
			justificacion = $"Sus gastos reales deducibles superan la deduccion plana del 15%, generando un ahorro estimado de {ahorro:N2}.";
		}
		else
		{
			recomendado = RegimenTributario.CAPITAL_INMOBILIARIO.ToString();
			ahorro = impuestoUtilidades - impuestoCapital;
			justificacion = "La deduccion plana del 15% resulta mas beneficiosa que sus gastos reales deducibles actuales.";
		}

		return (true, null, new SimulacionRegimenResponse
		{
			RentaBruta = rentaBruta,
			CapitalInmobiliario = new DetalleRegimenDto
			{
				Deduccion = deduccionCapital,
				RentaNeta = rentaNetaCapital,
				ImpuestoRenta = impuestoCapital,
				CuentaConComprobantesValidos = true
			},
			Utilidades = new DetalleRegimenDto
			{
				Deduccion = deduccionUtilidades,
				RentaNeta = rentaNetaUtilidades,
				ImpuestoRenta = impuestoUtilidades,
				CuentaConComprobantesValidos = tieneComprobantesValidos
			},
			RegimenRecomendado = recomendado,
			AhorroEstimado = ahorro,
			Justificacion = justificacion
		});
	}

	public async Task<(bool success, string? error, string? regimenActual)> CambiarRegimenAsync(
		CambiarRegimenTributarioRequest request)
	{
		var perfil = await repository.GetPerfilTributarioAsync(request.UsuarioId);
		if (perfil is null)
			return (false, "No existe un perfil tributario registrado para este usuario.", null);

		if (perfil.RegimenTributario == request.NuevoRegimen)
			return (false, $"El usuario ya se encuentra en el regimen {request.NuevoRegimen}.", null);

		if (request.NuevoRegimen == RegimenTributario.UTILIDADES)
		{
			var gastosValidos = await ObtenerGastosDeduciblesValidosAsync(request.UsuarioId, request.Anio, request.Mes);
			if (gastosValidos.Count == 0)
				return (false, "Debe adjuntar los comprobantes de gastos para justificar el cambio de regimen.", null);
		}

		var regimenAnterior = perfil.RegimenTributario;
		perfil.RegimenTributario = request.NuevoRegimen;

		await repository.AddAuditoriaCambioRegimenAsync(new AuditoriaOperacion
		{
			AuditId = Guid.NewGuid(),
			UsuarioId = request.UsuarioId,
			Operacion = OperacionAuditoria.CAMBIO_REGIMEN,
			TablaAfectada = nameof(Models.Entities.Identity.PerfilTributario),
			RegistroId = perfil.PerfilId,
			OldValues = regimenAnterior.ToString(),
			NewValues = request.NuevoRegimen.ToString(),
			Justificacion = $"Solicitado por el anfitrion para el periodo {request.Mes:00}/{request.Anio}, respaldado con comprobantes de gastos validados.",
			CreatedAt = DateTimeOffset.UtcNow
		});

		await repository.SaveChangesAsync();

		return (true, null, perfil.RegimenTributario.ToString());
	}

	private async Task<List<Models.Entities.Operations.Gasto>> ObtenerGastosDeduciblesValidosAsync(
		Guid usuarioId, short anio, short mes)
	{
		var gastos = await repository.GetGastosDeduciblesDelPeriodoAsync(usuarioId, anio, mes);
		return gastos.Where(g => g.EstadoValidacion == EstadoValidacion.VALIDO).ToList();
	}

	private static RentaCapitalResponse MapToResponse(CalculoFiscal calculo, short anio, short mes) => new()
	{
		CalculoId = calculo.CalculoId,
		Anio = anio,
		Mes = mes,
		RegimenAplicado = calculo.RegimenAplicado?.ToString() ?? string.Empty,
		RentaBruta = calculo.RentaBruta ?? 0m,
		DeduccionAplicada = calculo.DeduccionAplicada ?? 0m,
		RentaNeta = calculo.RentaNeta ?? 0m,
		TasaEfectiva = calculo.RentaBruta is > 0
			? (calculo.ImpuestoRenta ?? 0m) / calculo.RentaBruta.Value
			: 0m,
		ImpuestoRenta = calculo.ImpuestoRenta ?? 0m,
		RetencionesAcreditadas = calculo.RetencionesAcreditadas ?? 0m,
		MontoTotalAPagar = calculo.MontoTotalAPagar,
		BorradorGenerado = calculo.BorradorGenerado,
		FechaGeneracionBorrador = calculo.FechaGeneracionBorrador
	};
}
