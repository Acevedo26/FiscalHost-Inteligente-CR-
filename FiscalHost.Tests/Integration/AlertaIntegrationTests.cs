using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.Identity;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums.Communication;
using FiscalHost.Api.CR.Models.Enums.Identity;
using FiscalHost.Api.CR.Models.Enums.TaxIntelligence;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;

namespace FiscalHost.Tests.Integration;

public class AlertaIntegrationTests : IDisposable
{
	private readonly AppDbContext _db;
	private readonly AlertaService _sut;
	private readonly INotificacionService _notificacionService = Substitute.For<INotificacionService>();

	public AlertaIntegrationTests()
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;

		_db = new AppDbContext(options);

		var alertaRepository = new AlertaRepository(_db);
		var obligacionRepository = new ObligacionTributariaRepository(_db);
		var logger = Substitute.For<ILogger<AlertaService>>();

		_sut = new AlertaService(alertaRepository, obligacionRepository, _notificacionService, logger);
	}

	[Fact]
	public async Task GenerarYEnviarAlertas_FlujoCompleto_PersisteYEnviaCorrectamente()
	{
		var usuario = BuildUsuario();
		var fechaActual = DateOnly.FromDateTime(DateTime.UtcNow);
		var obligacion = BuildObligacion(usuario, fechaActual.AddDays(3));

		_db.Usuarios.Add(usuario);
		_db.ObligacionesTributarias.Add(obligacion);
		await _db.SaveChangesAsync();

		await _sut.GenerarAlertasVencimientoAsync(fechaActual);

		var alertaGenerada = Assert.Single(_db.Alertas);
		Assert.Equal(TipoAlerta.VENCIMIENTO_3_DIAS, alertaGenerada.TipoAlerta);
		Assert.Equal(EstadoNotificacion.PENDIENTE, alertaGenerada.Estado);

		await _sut.EnviarAlertasPendientesAsync(DateTimeOffset.UtcNow);

		await _notificacionService.Received(1).NotificarAsync(usuario.UsuarioId.ToString(), Arg.Any<string>());
		Assert.Equal(EstadoNotificacion.ENVIADA, alertaGenerada.Estado);
	}

	[Fact]
	public async Task GenerarAlertas_UsuarioConPreferenciaSoloPlataforma_NoEnviaCorreo()
	{
		var usuario = BuildUsuario(preferenciaCanal: CanalNotificacion.PLATAFORMA);
		var fechaActual = DateOnly.FromDateTime(DateTime.UtcNow);
		var obligacion = BuildObligacion(usuario, fechaActual.AddDays(1));

		_db.Usuarios.Add(usuario);
		_db.ObligacionesTributarias.Add(obligacion);
		await _db.SaveChangesAsync();

		await _sut.GenerarAlertasVencimientoAsync(fechaActual);
		var alertaGenerada = _db.Alertas.Single();
		Assert.Equal(CanalNotificacion.PLATAFORMA, alertaGenerada.Canal);

		await _sut.EnviarAlertasPendientesAsync(DateTimeOffset.UtcNow);

		await _notificacionService.DidNotReceive().NotificarAsync(Arg.Any<string>(), Arg.Any<string>());
		Assert.Equal(EstadoNotificacion.ENVIADA, alertaGenerada.Estado);
	}

	[Fact]
	public async Task GenerarAlertas_EjecutadoDosVeces_NoDuplicaAlertas()
	{
		var usuario = BuildUsuario();
		var fechaActual = DateOnly.FromDateTime(DateTime.UtcNow);
		var obligacion = BuildObligacion(usuario, fechaActual.AddDays(15));

		_db.Usuarios.Add(usuario);
		_db.ObligacionesTributarias.Add(obligacion);
		await _db.SaveChangesAsync();

		await _sut.GenerarAlertasVencimientoAsync(fechaActual);
		await _sut.GenerarAlertasVencimientoAsync(fechaActual);

		Assert.Single(_db.Alertas);
	}

	private static Usuario BuildUsuario(CanalNotificacion? preferenciaCanal = null) => new()
	{
		UsuarioId = Guid.NewGuid(),
		TipoIdentificacion = TipoIdentificacion.FISICA,
		NumeroIdentificacion = "1-1111-1111",
		NombreCompleto = "Usuario de Prueba",
		CorreoElectronico = $"{Guid.NewGuid()}@example.com",
		ContrasenaHash = "hash",
		Estado = EstadoUsuario.ACTIVO,
		RolPrincipal = RolUsuario.ANFITRION,
		PreferenciasNotificacion = preferenciaCanal.HasValue
			? $"{{\"canalAlertas\":\"{preferenciaCanal.Value}\"}}"
			: "{}",
	};

	private static ObligacionTributaria BuildObligacion(Usuario usuario, DateOnly fechaVencimiento) => new()
	{
		ObligacionId = Guid.NewGuid(),
		UsuarioId = usuario.UsuarioId,
		Usuario = usuario,
		PeriodoId = Guid.NewGuid(),
		TipoFormulario = TipoFormulario.D104,
		Descripcion = "IVA de prueba",
		MontoCapital = 10000,
		MontoMulta = 0,
		MontoInteresesAcumulados = 0,
		MontoTotalActualizado = 10000,
		FechaVencimiento = fechaVencimiento,
		Estado = EstadoObligacion.VIGENTE,
		HistorialIntereses = "{}",
	};

	public void Dispose() => _db.Dispose();
}