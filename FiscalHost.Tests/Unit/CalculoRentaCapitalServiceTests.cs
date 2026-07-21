using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.Entities.Audit;
using FiscalHost.Api.CR.Models.Entities.Identity;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Moq;
using Xunit;

namespace FiscalHost.Tests.Unit;

public class CalculoRentaCapitalServiceTests
{
	private readonly Mock<ICalculoRentaCapitalRepository> _repositoryMock;
	private readonly CalculoRentaCapitalService _service;

	private static readonly Guid UsuarioId = Guid.NewGuid();
	private static readonly Guid PeriodoId = Guid.NewGuid();

	public CalculoRentaCapitalServiceTests()
	{
		_repositoryMock = new Mock<ICalculoRentaCapitalRepository>();
		_service = new CalculoRentaCapitalService(_repositoryMock.Object);
	}

	private static PeriodoFiscal CrearPeriodoFiscal() => new()
	{
		PeriodoId = PeriodoId,
		Anio = 2026,
		Mes = 6,
		TipoFormulario = TipoFormulario.D125,
		TarifaRentaCapital = 15.00m,
		DeduccionPlanaCapital = 15.00m,
		NormativaAplicable = "Ley 7092, Titulo II, Renta de Capital Inmobiliario"
	};

	private static PerfilTributario CrearPerfil(RegimenTributario regimen) => new()
	{
		PerfilId = Guid.NewGuid(),
		UsuarioId = UsuarioId,
		RegimenTributario = regimen
	};

	private static Reserva CrearReserva(decimal montoColones) => new()
	{
		ReservaId = Guid.NewGuid(),
		UsuarioId = UsuarioId,
		FechaInicio = new DateTime(2026, 6, 1),
		FechaFin = new DateTime(2026, 6, 10),
		MontoBruto = montoColones,
		MontoColones = montoColones,
		Moneda = TipoMoneda.CRC,
		TipoCambio = 1,
		ClasificacionFiscal = ClasificacionFiscal.GRAVADO,
		PlataformaOrigen = PlataformaOrigen.AIRBNB,
		FuenteRegistro = FuenteRegistro.IMPORTACION_CSV,
		PeriodoFiscalAnio = 2026,
		PeriodoFiscalMes = 6,
		Estado = "EN_REVISION"
	};

	private static Gasto CrearGastoValido(decimal montoNeto) => new()
	{
		GastoId = Guid.NewGuid(),
		UsuarioId = UsuarioId,
		Proveedor = "Ferreteria Central",
		FechaEmision = new DateOnly(2026, 6, 5),
		MontoTotal = montoNeto,
		MontoIvaSoportado = 0m,
		MontoNeto = montoNeto,
		MontoColones = montoNeto,
		Moneda = TipoMoneda.CRC,
		TipoGasto = "MANTENIMIENTO",
		EsDeducibleRenta = true,
		EsCreditoFiscalValido = false,
		EstadoValidacion = EstadoValidacion.VALIDO,
		PeriodoFiscalAnio = 2026,
		PeriodoFiscalMes = 6,
		FuenteRegistro = FuenteRegistro.MANUAL
	};

	[Fact]
	public async Task CalcularAsync_PerfilNoExiste_RetornaError()
	{
		_repositoryMock
			.Setup(r => r.GetPerfilTributarioAsync(UsuarioId))
			.ReturnsAsync((PerfilTributario?)null);

		var (success, error, data) = await _service.CalcularAsync(
			new CalcularRentaCapitalRequest { UsuarioId = UsuarioId, Anio = 2026, Mes = 6 });

		Assert.False(success);
		Assert.Null(data);
		Assert.Contains("perfil tributario", error);
	}

	[Fact]
	public async Task CalcularAsync_PeriodoNoConfigurado_RetornaError()
	{
		_repositoryMock
			.Setup(r => r.GetPerfilTributarioAsync(UsuarioId))
			.ReturnsAsync(CrearPerfil(RegimenTributario.CAPITAL_INMOBILIARIO));

		_repositoryMock
			.Setup(r => r.GetPeriodoFiscalAsync(2026, 6, TipoFormulario.D125))
			.ReturnsAsync((PeriodoFiscal?)null);

		var (success, error, data) = await _service.CalcularAsync(
			new CalcularRentaCapitalRequest { UsuarioId = UsuarioId, Anio = 2026, Mes = 6 });

		Assert.False(success);
		Assert.Null(data);
		Assert.Contains("periodo fiscal", error);
	}

	[Fact]
	public async Task CalcularAsync_SinReservasEnElPeriodo_RetornaError()
	{
		_repositoryMock
			.Setup(r => r.GetPerfilTributarioAsync(UsuarioId))
			.ReturnsAsync(CrearPerfil(RegimenTributario.CAPITAL_INMOBILIARIO));

		_repositoryMock
			.Setup(r => r.GetPeriodoFiscalAsync(2026, 6, TipoFormulario.D125))
			.ReturnsAsync(CrearPeriodoFiscal());

		_repositoryMock
			.Setup(r => r.GetReservasDelPeriodoAsync(UsuarioId, 2026, 6))
			.ReturnsAsync(new List<Reserva>());

		var (success, error, data) = await _service.CalcularAsync(
			new CalcularRentaCapitalRequest { UsuarioId = UsuarioId, Anio = 2026, Mes = 6 });

		Assert.False(success);
		Assert.Null(data);
		Assert.Contains("No hay ingresos registrados", error);
	}

	[Fact]
	public async Task CalcularAsync_RegimenCapitalInmobiliario_AplicaTasaEfectivaDe12_75Porciento()
	{
		_repositoryMock
			.Setup(r => r.GetPerfilTributarioAsync(UsuarioId))
			.ReturnsAsync(CrearPerfil(RegimenTributario.CAPITAL_INMOBILIARIO));

		_repositoryMock
			.Setup(r => r.GetPeriodoFiscalAsync(2026, 6, TipoFormulario.D125))
			.ReturnsAsync(CrearPeriodoFiscal());

		_repositoryMock
			.Setup(r => r.GetReservasDelPeriodoAsync(UsuarioId, 2026, 6))
			.ReturnsAsync(new List<Reserva> { CrearReserva(1_000_000m) });

		_repositoryMock
			.Setup(r => r.GetCalculoExistenteAsync(UsuarioId, PeriodoId, TipoFormulario.D125))
			.ReturnsAsync((CalculoFiscal?)null);

		var (success, error, data) = await _service.CalcularAsync(
			new CalcularRentaCapitalRequest { UsuarioId = UsuarioId, Anio = 2026, Mes = 6 });

		Assert.True(success);
		Assert.Null(error);
		Assert.NotNull(data);
		Assert.Equal(1_000_000m, data!.RentaBruta);
		Assert.Equal(150_000m, data.DeduccionAplicada);   // 15% de deduccion plana
		Assert.Equal(850_000m, data.RentaNeta);
		Assert.Equal(127_500m, data.ImpuestoRenta);        // 850,000 * 15% = 12.75% efectivo sobre el bruto
		Assert.Equal(0.1275m, data.TasaEfectiva);

		_repositoryMock.Verify(r => r.AddCalculoAsync(It.IsAny<CalculoFiscal>()), Times.Once);
		_repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
	}

	[Fact]
	public async Task CalcularAsync_RegimenUtilidadesSinComprobantesValidos_RetornaError()
	{
		_repositoryMock
			.Setup(r => r.GetPerfilTributarioAsync(UsuarioId))
			.ReturnsAsync(CrearPerfil(RegimenTributario.UTILIDADES));

		_repositoryMock
			.Setup(r => r.GetPeriodoFiscalAsync(2026, 6, TipoFormulario.D125))
			.ReturnsAsync(CrearPeriodoFiscal());

		_repositoryMock
			.Setup(r => r.GetReservasDelPeriodoAsync(UsuarioId, 2026, 6))
			.ReturnsAsync(new List<Reserva> { CrearReserva(1_000_000m) });

		_repositoryMock
			.Setup(r => r.GetGastosDeduciblesDelPeriodoAsync(UsuarioId, 2026, 6))
			.ReturnsAsync(new List<Gasto>());

		var (success, error, data) = await _service.CalcularAsync(
			new CalcularRentaCapitalRequest { UsuarioId = UsuarioId, Anio = 2026, Mes = 6 });

		Assert.False(success);
		Assert.Null(data);
		Assert.Contains("comprobantes de gastos", error);
		_repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
	}

	[Fact]
	public async Task CalcularAsync_RegimenUtilidadesConComprobantesValidos_UsaGastosRealesComoDeduccion()
	{
		_repositoryMock
			.Setup(r => r.GetPerfilTributarioAsync(UsuarioId))
			.ReturnsAsync(CrearPerfil(RegimenTributario.UTILIDADES));

		_repositoryMock
			.Setup(r => r.GetPeriodoFiscalAsync(2026, 6, TipoFormulario.D125))
			.ReturnsAsync(CrearPeriodoFiscal());

		_repositoryMock
			.Setup(r => r.GetReservasDelPeriodoAsync(UsuarioId, 2026, 6))
			.ReturnsAsync(new List<Reserva> { CrearReserva(1_000_000m) });

		_repositoryMock
			.Setup(r => r.GetGastosDeduciblesDelPeriodoAsync(UsuarioId, 2026, 6))
			.ReturnsAsync(new List<Gasto> { CrearGastoValido(300_000m) });

		_repositoryMock
			.Setup(r => r.GetCalculoExistenteAsync(UsuarioId, PeriodoId, TipoFormulario.D125))
			.ReturnsAsync((CalculoFiscal?)null);

		var (success, _, data) = await _service.CalcularAsync(
			new CalcularRentaCapitalRequest { UsuarioId = UsuarioId, Anio = 2026, Mes = 6 });

		Assert.True(success);
		Assert.Equal(300_000m, data!.DeduccionAplicada);
		Assert.Equal(700_000m, data.RentaNeta);
		Assert.Equal(105_000m, data.ImpuestoRenta); // 700,000 * 15%
	}

	[Fact]
	public async Task SimularAsync_SinComprobantesValidos_RecomiendaCapitalInmobiliario()
	{
		_repositoryMock
			.Setup(r => r.GetPeriodoFiscalAsync(2026, 6, TipoFormulario.D125))
			.ReturnsAsync(CrearPeriodoFiscal());

		_repositoryMock
			.Setup(r => r.GetReservasDelPeriodoAsync(UsuarioId, 2026, 6))
			.ReturnsAsync(new List<Reserva> { CrearReserva(1_000_000m) });

		_repositoryMock
			.Setup(r => r.GetGastosDeduciblesDelPeriodoAsync(UsuarioId, 2026, 6))
			.ReturnsAsync(new List<Gasto>());

		var (success, _, data) = await _service.SimularAsync(
			new SimularRegimenRequest { UsuarioId = UsuarioId, Anio = 2026, Mes = 6 });

		Assert.True(success);
		Assert.Equal(RegimenTributario.CAPITAL_INMOBILIARIO.ToString(), data!.RegimenRecomendado);
		Assert.False(data.Utilidades.CuentaConComprobantesValidos);
	}

	[Fact]
	public async Task SimularAsync_GastosRealesMayoresQueDeduccionPlana_RecomiendaUtilidades()
	{
		_repositoryMock
			.Setup(r => r.GetPeriodoFiscalAsync(2026, 6, TipoFormulario.D125))
			.ReturnsAsync(CrearPeriodoFiscal());

		_repositoryMock
			.Setup(r => r.GetReservasDelPeriodoAsync(UsuarioId, 2026, 6))
			.ReturnsAsync(new List<Reserva> { CrearReserva(1_000_000m) });

		// Gastos reales (400,000) > deduccion plana del 15% (150,000)
		_repositoryMock
			.Setup(r => r.GetGastosDeduciblesDelPeriodoAsync(UsuarioId, 2026, 6))
			.ReturnsAsync(new List<Gasto> { CrearGastoValido(400_000m) });

		var (success, _, data) = await _service.SimularAsync(
			new SimularRegimenRequest { UsuarioId = UsuarioId, Anio = 2026, Mes = 6 });

		Assert.True(success);
		Assert.Equal(RegimenTributario.UTILIDADES.ToString(), data!.RegimenRecomendado);
		Assert.True(data.AhorroEstimado > 0);
	}

	[Fact]
	public async Task CambiarRegimenAsync_YaEstaEnElRegimenSolicitado_RetornaError()
	{
		_repositoryMock
			.Setup(r => r.GetPerfilTributarioAsync(UsuarioId))
			.ReturnsAsync(CrearPerfil(RegimenTributario.UTILIDADES));

		var (success, error, _) = await _service.CambiarRegimenAsync(new CambiarRegimenTributarioRequest
		{
			UsuarioId = UsuarioId,
			NuevoRegimen = RegimenTributario.UTILIDADES,
			Anio = 2026,
			Mes = 6
		});

		Assert.False(success);
		Assert.Contains("ya se encuentra", error);
	}

	[Fact]
	public async Task CambiarRegimenAsync_AUtilidadesSinComprobantesValidos_RetornaError()
	{
		_repositoryMock
			.Setup(r => r.GetPerfilTributarioAsync(UsuarioId))
			.ReturnsAsync(CrearPerfil(RegimenTributario.CAPITAL_INMOBILIARIO));

		_repositoryMock
			.Setup(r => r.GetGastosDeduciblesDelPeriodoAsync(UsuarioId, 2026, 6))
			.ReturnsAsync(new List<Gasto>());

		var (success, error, _) = await _service.CambiarRegimenAsync(new CambiarRegimenTributarioRequest
		{
			UsuarioId = UsuarioId,
			NuevoRegimen = RegimenTributario.UTILIDADES,
			Anio = 2026,
			Mes = 6
		});

		Assert.False(success);
		Assert.Contains("comprobantes de gastos", error);
	}

	[Fact]
	public async Task CambiarRegimenAsync_ConComprobantesValidos_ActualizaPerfilYRegistraAuditoria()
	{
		var perfil = CrearPerfil(RegimenTributario.CAPITAL_INMOBILIARIO);

		_repositoryMock
			.Setup(r => r.GetPerfilTributarioAsync(UsuarioId))
			.ReturnsAsync(perfil);

		_repositoryMock
			.Setup(r => r.GetGastosDeduciblesDelPeriodoAsync(UsuarioId, 2026, 6))
			.ReturnsAsync(new List<Gasto> { CrearGastoValido(300_000m) });

		var (success, error, regimenActual) = await _service.CambiarRegimenAsync(new CambiarRegimenTributarioRequest
		{
			UsuarioId = UsuarioId,
			NuevoRegimen = RegimenTributario.UTILIDADES,
			Anio = 2026,
			Mes = 6
		});

		Assert.True(success);
		Assert.Null(error);
		Assert.Equal(RegimenTributario.UTILIDADES.ToString(), regimenActual);
		Assert.Equal(RegimenTributario.UTILIDADES, perfil.RegimenTributario);

		_repositoryMock.Verify(r => r.AddAuditoriaCambioRegimenAsync(It.IsAny<AuditoriaOperacion>()), Times.Once);
		_repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
	}
}
