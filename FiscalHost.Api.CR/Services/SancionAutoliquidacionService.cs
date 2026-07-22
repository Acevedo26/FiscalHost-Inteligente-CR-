using System.Text.Json;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public interface ISancionAutoliquidacionService
{
	Task<(bool success, string? error, SancionResponse? data)> CalcularAsync(CalcularSancionRequest request);
}

public class SancionAutoliquidacionService(
	ISancionAutoliquidacionRepository repository)
	: ISancionAutoliquidacionService
{
	private const decimal PorcentajeReduccionArt88 = 80.00m;

	public async Task<(bool success, string? error, SancionResponse? data)> CalcularAsync(
		CalcularSancionRequest request)
	{
		var obligacion = await repository.GetObligacionPendienteAsync(request.UsuarioId, request.ObligacionId);
		if (obligacion is null || obligacion.MontoCapital <= 0)
			return (false, "No hay deuda registrada para este periodo. Verifique los datos historicos.", null);

		var anio = (short)obligacion.FechaVencimiento.Year;
		var periodo = await repository.GetPeriodoConSalarioBaseAsync(anio, TipoFormulario.D176);
		if (periodo is null || periodo.SalarioBaseVigente is null)
			return (false,
				$"No hay salario base configurado para el {anio}. Debe registrarse primero.",
				null);

		var perfil = await repository.GetPerfilTributarioAsync(request.UsuarioId);

		var esInscripcionTardia = perfil is { FechaInicioActividad: not null, FechaInscripcionHacienda: not null }
								   && perfil.FechaInscripcionHacienda > perfil.FechaInicioActividad;

		var tipoSancion = esInscripcionTardia ? "INSCRIPCION_TARDIA" : "OMISION_DECLARACION";

		// Art. 79 CNPT: multa de medio salario base para inscripcion tardia.
		var multaBase = periodo.SalarioBaseVigente.Value * 0.5m;

		// Art. 88 CNPT: reduccion del 80% por subsanacion voluntaria y espontanea.
		var montoReduccion = multaBase * (PorcentajeReduccionArt88 / 100m);
		var multaReducida = multaBase - montoReduccion;

		var interesesAcumulados = obligacion.MontoInteresesAcumulados;

		var descripcion = esInscripcionTardia
			? "Multa por inscripcion tardia ante la Administracion Tributaria (Art. 79 CNPT), reducida un 80% por subsanacion voluntaria (Art. 88 CNPT)."
			: "Multa por omision de declaracion (Art. 79 CNPT), reducida un 80% por subsanacion voluntaria (Art. 88 CNPT).";

		var sancionExistente = await repository.GetSancionExistenteAsync(request.ObligacionId);
		var sancion = sancionExistente
					  ?? new SancionAutoliquidacion { SancionId = Guid.NewGuid(), UsuarioId = request.UsuarioId, ObligacionId = request.ObligacionId };

		if (sancionExistente is null)
			await repository.AddSancionAsync(sancion);

		sancion.UsuarioId = request.UsuarioId;
		sancion.ObligacionId = request.ObligacionId;
		sancion.TipoSancion = tipoSancion;
		sancion.FechaOmision = DateTime.SpecifyKind(obligacion.FechaVencimiento.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
		sancion.MontoBaseAdeudado = obligacion.MontoCapital;
		sancion.MultaBaseCalculada = multaBase;
		sancion.PorcentajeReduccion = PorcentajeReduccionArt88;
		sancion.MontoReduccion = montoReduccion;
		sancion.MultaReducida = multaReducida;
		sancion.InteresesAcumulados = interesesAcumulados;
		sancion.Estado = "CALCULADO";
		sancion.Descripcion = descripcion;
		sancion.DetalleCalculo = JsonSerializer.Serialize(new
		{
			tipoFormularioGenerado = "D176",
			salarioBaseVigente = periodo.SalarioBaseVigente,
			formula = "multaBase = salarioBaseVigente * 0.5; multaReducida = multaBase * (1 - 0.80)",
			montoCapital = obligacion.MontoCapital,
			interesesAcumulados
		});

		obligacion.MontoMulta = multaReducida;
		obligacion.UpdatedAt = DateTimeOffset.UtcNow;

		await repository.SaveChangesAsync();

		return (true, null, new SancionResponse
		{
			SancionId = sancion.SancionId,
			ObligacionId = request.ObligacionId,
			TipoSancion = tipoSancion,
			MontoBaseAdeudado = obligacion.MontoCapital,
			MultaBaseCalculada = multaBase,
			PorcentajeReduccion = PorcentajeReduccionArt88,
			MontoReduccion = montoReduccion,
			MultaReducida = multaReducida,
			InteresesAcumulados = interesesAcumulados,
			MontoTotalPagar = sancion.MontoTotalPagar,
			TipoFormularioGenerado = "D176",
			Estado = sancion.Estado,
			Descripcion = descripcion
		});
	}
}
