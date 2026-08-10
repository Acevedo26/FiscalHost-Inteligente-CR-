using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FiscalHost.Api.CR.Models.Entities.Communication;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums.Communication;
using FiscalHost.Api.CR.Models.Enums.TaxIntelligence;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using NSubstitute;

namespace FiscalHost.Tests;

public class AlertaServiceTests
{
	private readonly IAlertaRepository _alertaRepository = Substitute.For<IAlertaRepository>();
	private readonly IObligacionTributariaRepository _obligacionRepository = Substitute.For<IObligacionTributariaRepository>();
	private readonly INotificacionService _notificacionService = Substitute.For<INotificacionService>();
	private readonly ILogger<AlertaService> _logger = Substitute.For<ILogger<AlertaService>>();

	private readonly AlertaService _sut;

	public AlertaServiceTests()
	{
		_sut = new AlertaService(_alertaRepository, _obligacionRepository, _notificacionService, _logger);
	}

	[Fact]
	public async Task GenerarAlertas_ObligacionA7Dias_CreaAlertaConTipoCorrecto()
	{
		var fechaActual = new DateOnly(2026, 8, 1);
		var obligacion = BuildObligacion(fechaVencimiento: fechaActual.AddDays(7));

		_obligacionRepository.GetProximasAVencerAsync(fechaActual, 15)
			.Returns([obligacion]);
		_alertaRepository.ExisteAlertaParaObligacionAsync(obligacion.ObligacionId, TipoAlerta.VENCIMIENTO_7_DIAS)
			.Returns(false);

		await _sut.GenerarAlertasVencimientoAsync(fechaActual);

		await _alertaRepository.Received(1).AddRangeAsync(Arg.Is<IEnumerable<Alerta>>(
			alertas => HasSingleAlertaOfType(alertas, TipoAlerta.VENCIMIENTO_7_DIAS)));
		await _alertaRepository.Received(1).SaveChangesAsync();
	}

	[Fact]
	public async Task GenerarAlertas_AlertaYaExiste_NoDuplica()
	{
		var fechaActual = new DateOnly(2026, 8, 1);
		var obligacion = BuildObligacion(fechaVencimiento: fechaActual.AddDays(3));

		_obligacionRepository.GetProximasAVencerAsync(fechaActual, 15)
			.Returns([obligacion]);
		_alertaRepository.ExisteAlertaParaObligacionAsync(obligacion.ObligacionId, TipoAlerta.VENCIMIENTO_3_DIAS)
			.Returns(true);

		await _sut.GenerarAlertasVencimientoAsync(fechaActual);

		await _alertaRepository.DidNotReceive().AddRangeAsync(Arg.Any<IEnumerable<Alerta>>());
	}

	[Fact]
	public async Task GenerarAlertas_DiasRestantesFueraDeUmbrales_NoGeneraAlerta()
	{
		var fechaActual = new DateOnly(2026, 8, 1);
		// 12 días no corresponde a ningún umbral (15, 10, 7, 3, 1)
		var obligacion = BuildObligacion(fechaVencimiento: fechaActual.AddDays(12));

		_obligacionRepository.GetProximasAVencerAsync(fechaActual, 15)
			.Returns([obligacion]);

		await _sut.GenerarAlertasVencimientoAsync(fechaActual);

		await _alertaRepository.DidNotReceive().AddRangeAsync(Arg.Any<IEnumerable<Alerta>>());
	}

	[Fact]
	public async Task EnviarAlertasPendientes_EnvioExitoso_ActualizaEstadoAEnviada()
	{
		var alerta = BuildAlerta();
		_alertaRepository.GetPendientesParaEnvioAsync(Arg.Any<DateTimeOffset>())
			.Returns([alerta]);

		await _sut.EnviarAlertasPendientesAsync(DateTimeOffset.UtcNow);

		await _notificacionService.Received(1).NotificarAsync(alerta.UsuarioId.ToString(), alerta.Mensaje);
		Assert.Equal(EstadoNotificacion.ENVIADA, alerta.Estado);
		Assert.NotNull(alerta.FechaEnvio);
		Assert.Equal(1, alerta.IntentosEnvio);
		await _alertaRepository.Received(1).SaveChangesAsync();
	}

	[Fact]
	public async Task EnviarAlertasPendientes_FalloTrasTresIntentos_MarcaComoFallida()
	{
		var alerta = BuildAlerta();
		alerta.IntentosEnvio = 2;
		_alertaRepository.GetPendientesParaEnvioAsync(Arg.Any<DateTimeOffset>())
			.Returns([alerta]);
		_notificacionService.NotificarAsync(Arg.Any<string>(), Arg.Any<string>())
			.Returns(Task.FromException(new InvalidOperationException("Fallo simulado de envío")));

		await _sut.EnviarAlertasPendientesAsync(DateTimeOffset.UtcNow);

		Assert.Equal(EstadoNotificacion.FALLIDA, alerta.Estado);
		Assert.Equal(3, alerta.IntentosEnvio);
		Assert.NotNull(alerta.ErrorEnvio);
	}

	[Fact]
	public async Task MarcarComoLeida_AlertaExistente_ActualizaFechaLecturaYEstado()
	{
		var alerta = BuildAlerta();
		alerta.Estado = EstadoNotificacion.ENVIADA;
		_alertaRepository.GetByIdAsync(alerta.AlertaId).Returns(alerta);

		var resultado = await _sut.MarcarComoLeidaAsync(alerta.AlertaId);

		Assert.NotNull(resultado);
		Assert.NotNull(alerta.FechaLectura);
		Assert.Equal(EstadoNotificacion.LEIDA, alerta.Estado);
		await _alertaRepository.Received(1).SaveChangesAsync();
	}

	[Fact]
	public async Task MarcarComoLeida_AlertaInexistente_RetornaNull()
	{
		_alertaRepository.GetByIdAsync(Arg.Any<Guid>()).Returns((Alerta?)null);

		var resultado = await _sut.MarcarComoLeidaAsync(Guid.NewGuid());

		Assert.Null(resultado);
	}

	private static bool HasSingleAlertaOfType(IEnumerable<Alerta> alertas, TipoAlerta tipo)
	{
		var list = new List<Alerta>(alertas);
		return list.Count == 1 && list[0].TipoAlerta == tipo;
	}

	private static ObligacionTributaria BuildObligacion(DateOnly fechaVencimiento) => new()
	{
		ObligacionId = Guid.NewGuid(),
		UsuarioId = Guid.NewGuid(),
		PeriodoId = Guid.NewGuid(),
		TipoFormulario = TipoFormulario.D104,
		Descripcion = "IVA agosto 2026",
		MontoCapital = 50000,
		MontoMulta = 0,
		MontoInteresesAcumulados = 0,
		MontoTotalActualizado = 50000,
		FechaVencimiento = fechaVencimiento,
		Estado = EstadoObligacion.VIGENTE,
		HistorialIntereses = "{}",
	};

	private static Alerta BuildAlerta() => new()
	{
		AlertaId = Guid.NewGuid(),
		UsuarioId = Guid.NewGuid(),
		TipoAlerta = TipoAlerta.VENCIMIENTO_3_DIAS,
		Titulo = "Vencimiento próximo",
		Mensaje = "Su obligación vence pronto.",
		Prioridad = 2,
		Canal = CanalNotificacion.AMBOS,
		Estado = EstadoNotificacion.PENDIENTE,
		AccionSugerida = "{}",
		FechaProgramada = DateTimeOffset.UtcNow.AddMinutes(-5),
		IntentosEnvio = 0,
	};
}