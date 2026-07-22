using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.Entities.Identity;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Moq;
using Xunit;

namespace FiscalHost.Tests.Unit;

public class SancionAutoliquidacionServiceTests
{
	private readonly Mock<ISancionAutoliquidacionRepository> _repositoryMock;
	private readonly SancionAutoliquidacionService _service;

	private SancionAutoliquidacion? _sancionActiva;

	private static readonly Guid UsuarioId = Guid.NewGuid();
	private static readonly Guid ObligacionId = Guid.NewGuid();

	public SancionAutoliquidacionServiceTests()
	{
		_repositoryMock = new Mock<ISancionAutoliquidacionRepository>();
		_service = new SancionAutoliquidacionService(_repositoryMock.Object);

		_repositoryMock
			.Setup(r => r.AddSancionAsync(It.IsAny<SancionAutoliquidacion>()))
			.Callback<SancionAutoliquidacion>(s => _sancionActiva = s)
			.Returns(Task.CompletedTask);

		_repositoryMock
			.Setup(r => r.SaveChangesAsync())
			.Returns(() =>
			{
				if (_sancionActiva is not null)
					_sancionActiva.MontoTotalPagar = _sancionActiva.MultaReducida + _sancionActiva.InteresesAcumulados;

				return Task.CompletedTask;
			});
	}

	private static ObligacionTributaria CrearObligacion(decimal montoCapital, decimal interesesAcumulados = 0m) => new()
	{
		ObligacionId = ObligacionId,
		UsuarioId = UsuarioId,
		PeriodoId = Guid.NewGuid(),
		TipoFormulario = TipoFormulario.D104,
		Estado = EstadoObligacion.VENCIDA,
		Descripcion = "IVA enero 2025",
		FechaVencimiento = new DateOnly(2025, 2, 15),
		MontoCapital = montoCapital,
		MontoInteresesAcumulados = interesesAcumulados,
		MontoMulta = 0m,
		MontoTotalActualizado = montoCapital,
		HistorialIntereses = "[]",
		CreatedAt = DateTimeOffset.UtcNow,
		UpdatedAt = DateTimeOffset.UtcNow
	};

	private static PeriodoFiscal CrearPeriodoConSalarioBase(decimal salarioBase) => new()
	{
		PeriodoId = Guid.NewGuid(),
		Anio = 2025,
		Mes = 12,
		TipoFormulario = TipoFormulario.D176,
		SalarioBaseVigente = salarioBase,
		NormativaAplicable = "Art. 79 y 88 CNPT"
	};

	private static PerfilTributario CrearPerfil(DateTime? fechaInicioActividad, DateTime? fechaInscripcionHacienda) => new()
	{
		PerfilId = Guid.NewGuid(),
		UsuarioId = UsuarioId,
		FechaInicioActividad = fechaInicioActividad,
		FechaInscripcionHacienda = fechaInscripcionHacienda
	};

	[Fact]
	public async Task CalcularAsync_ObligacionNoExiste_RetornaError()
	{
		_repositoryMock
			.Setup(r => r.GetObligacionPendienteAsync(UsuarioId, ObligacionId))
			.ReturnsAsync((ObligacionTributaria?)null);

		var (success, error, data) = await _service.CalcularAsync(
			new CalcularSancionRequest { UsuarioId = UsuarioId, ObligacionId = ObligacionId });

		Assert.False(success);
		Assert.Null(data);
		Assert.Contains("No hay deuda registrada", error);
	}

	[Fact]
	public async Task CalcularAsync_ObligacionSinMontoCapital_RetornaError()
	{
		_repositoryMock
			.Setup(r => r.GetObligacionPendienteAsync(UsuarioId, ObligacionId))
			.ReturnsAsync(CrearObligacion(0m));

		var (success, error, data) = await _service.CalcularAsync(
			new CalcularSancionRequest { UsuarioId = UsuarioId, ObligacionId = ObligacionId });

		Assert.False(success);
		Assert.Null(data);
		Assert.Contains("No hay deuda registrada", error);
	}

	[Fact]
	public async Task CalcularAsync_SinSalarioBaseConfigurado_RetornaError()
	{
		_repositoryMock
			.Setup(r => r.GetObligacionPendienteAsync(UsuarioId, ObligacionId))
			.ReturnsAsync(CrearObligacion(100_000m));

		_repositoryMock
			.Setup(r => r.GetPeriodoConSalarioBaseAsync(2025, TipoFormulario.D176))
			.ReturnsAsync((PeriodoFiscal?)null);

		var (success, error, data) = await _service.CalcularAsync(
			new CalcularSancionRequest { UsuarioId = UsuarioId, ObligacionId = ObligacionId });

		Assert.False(success);
		Assert.Null(data);
		Assert.Contains("salario base", error);
	}

	[Fact]
	public async Task CalcularAsync_InscripcionTardia_AplicaMultaDeMedioSalarioBaseConReduccion80Porciento()
	{
		_repositoryMock
			.Setup(r => r.GetObligacionPendienteAsync(UsuarioId, ObligacionId))
			.ReturnsAsync(CrearObligacion(100_000m));

		_repositoryMock
			.Setup(r => r.GetPeriodoConSalarioBaseAsync(2025, TipoFormulario.D176))
			.ReturnsAsync(CrearPeriodoConSalarioBase(462_200m));

		_repositoryMock
			.Setup(r => r.GetPerfilTributarioAsync(UsuarioId))
			.ReturnsAsync(CrearPerfil(
				fechaInicioActividad: new DateTime(2025, 1, 1),
				fechaInscripcionHacienda: new DateTime(2025, 3, 1))); // se inscribio despues de iniciar

		_repositoryMock
			.Setup(r => r.GetSancionExistenteAsync(ObligacionId))
			.ReturnsAsync((SancionAutoliquidacion?)null);

		var (success, error, data) = await _service.CalcularAsync(
			new CalcularSancionRequest { UsuarioId = UsuarioId, ObligacionId = ObligacionId });

		Assert.True(success);
		Assert.Null(error);
		Assert.NotNull(data);
		Assert.Equal("INSCRIPCION_TARDIA", data!.TipoSancion);
		Assert.Equal(231_100m, data.MultaBaseCalculada);   // 462,200 * 0.5
		Assert.Equal(80.00m, data.PorcentajeReduccion);
		Assert.Equal(184_880m, data.MontoReduccion);        // 231,100 * 0.80
		Assert.Equal(46_220m, data.MultaReducida);          // 231,100 * 0.20
		Assert.Equal(46_220m, data.MontoTotalPagar);        // multa_reducida + intereses (0) - columna generada, sin el capital
		Assert.Equal("D176", data.TipoFormularioGenerado);

		_repositoryMock.Verify(r => r.AddSancionAsync(It.IsAny<SancionAutoliquidacion>()), Times.Once);
		_repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
	}

	[Fact]
	public async Task CalcularAsync_SinInscripcionTardia_ClasificaComoOmisionDeclaracion()
	{
		_repositoryMock
			.Setup(r => r.GetObligacionPendienteAsync(UsuarioId, ObligacionId))
			.ReturnsAsync(CrearObligacion(100_000m));

		_repositoryMock
			.Setup(r => r.GetPeriodoConSalarioBaseAsync(2025, TipoFormulario.D176))
			.ReturnsAsync(CrearPeriodoConSalarioBase(462_200m));

		_repositoryMock
			.Setup(r => r.GetPerfilTributarioAsync(UsuarioId))
			.ReturnsAsync(CrearPerfil(
				fechaInicioActividad: new DateTime(2025, 1, 1),
				fechaInscripcionHacienda: new DateTime(2025, 1, 1))); // inscripcion a tiempo

		_repositoryMock
			.Setup(r => r.GetSancionExistenteAsync(ObligacionId))
			.ReturnsAsync((SancionAutoliquidacion?)null);

		var (success, _, data) = await _service.CalcularAsync(
			new CalcularSancionRequest { UsuarioId = UsuarioId, ObligacionId = ObligacionId });

		Assert.True(success);
		Assert.Equal("OMISION_DECLARACION", data!.TipoSancion);
	}

	[Fact]
	public async Task CalcularAsync_ConInteresesAcumuladosExistentes_LosIncluyeEnElMontoTotal()
	{
		_repositoryMock
			.Setup(r => r.GetObligacionPendienteAsync(UsuarioId, ObligacionId))
			.ReturnsAsync(CrearObligacion(100_000m, interesesAcumulados: 5_000m));

		_repositoryMock
			.Setup(r => r.GetPeriodoConSalarioBaseAsync(2025, TipoFormulario.D176))
			.ReturnsAsync(CrearPeriodoConSalarioBase(462_200m));

		_repositoryMock
			.Setup(r => r.GetPerfilTributarioAsync(UsuarioId))
			.ReturnsAsync((PerfilTributario?)null);

		_repositoryMock
			.Setup(r => r.GetSancionExistenteAsync(ObligacionId))
			.ReturnsAsync((SancionAutoliquidacion?)null);

		var (success, _, data) = await _service.CalcularAsync(
			new CalcularSancionRequest { UsuarioId = UsuarioId, ObligacionId = ObligacionId });

		Assert.True(success);
		Assert.Equal(5_000m, data!.InteresesAcumulados);
		Assert.Equal(46_220m + 5_000m, data.MontoTotalPagar);   // multa_reducida + intereses, sin el capital
	}

	[Fact]
	public async Task CalcularAsync_SiYaExisteUnaSancionParaLaObligacion_LaActualizaEnVezDeCrearOtra()
	{
		var sancionExistente = new SancionAutoliquidacion
		{
			SancionId = Guid.NewGuid(),
			UsuarioId = UsuarioId,
			ObligacionId = ObligacionId
		};
		_sancionActiva = sancionExistente;

		_repositoryMock
			.Setup(r => r.GetObligacionPendienteAsync(UsuarioId, ObligacionId))
			.ReturnsAsync(CrearObligacion(100_000m));

		_repositoryMock
			.Setup(r => r.GetPeriodoConSalarioBaseAsync(2025, TipoFormulario.D176))
			.ReturnsAsync(CrearPeriodoConSalarioBase(462_200m));

		_repositoryMock
			.Setup(r => r.GetPerfilTributarioAsync(UsuarioId))
			.ReturnsAsync((PerfilTributario?)null);

		_repositoryMock
			.Setup(r => r.GetSancionExistenteAsync(ObligacionId))
			.ReturnsAsync(sancionExistente);

		var (success, _, data) = await _service.CalcularAsync(
			new CalcularSancionRequest { UsuarioId = UsuarioId, ObligacionId = ObligacionId });

		Assert.True(success);
		Assert.Equal(sancionExistente.SancionId, data!.SancionId);
		Assert.Equal(46_220m, data.MontoTotalPagar);

		_repositoryMock.Verify(r => r.AddSancionAsync(It.IsAny<SancionAutoliquidacion>()), Times.Never);
		_repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
	}
}
