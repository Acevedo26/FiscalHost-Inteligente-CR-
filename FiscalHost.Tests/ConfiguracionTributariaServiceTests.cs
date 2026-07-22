using FiscalHost.Api.CR.Models.DTOs;
using FiscalHost.Api.CR.Models.Entities;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using NSubstitute;

namespace FiscalHost.Tests;

public class ConfiguracionTributariaServiceTests
{
    private readonly IActividadEconomicaRepository _actividadRepo = Substitute.For<IActividadEconomicaRepository>();
    private readonly IConfiguracionTributariaRepository _configRepo = Substitute.For<IConfiguracionTributariaRepository>();
    private readonly ConfiguracionTributariaService _sut;

    private static readonly CatalogoActividadEconomica ActividadValida = new()
    {
        Codigo = "551001", Descripcion = "Hoteles y alojamiento turístico", Vigente = true
    };

    public ConfiguracionTributariaServiceTests()
    {
        _sut = new ConfiguracionTributariaService(_configRepo, _actividadRepo);
    }

    // Escenario: Código de actividad inválido
    [Fact]
    public async Task GuardarConfiguracion_CodigoInvalido_RetornaError()
    {
        _actividadRepo.GetByCodigoAsync("999999").Returns((CatalogoActividadEconomica?)null);
        var request = BuildRequest(codigoActividad: "999999");

        var (success, error, data) = await _sut.GuardarConfiguracionAsync(request);

        Assert.False(success);
        Assert.Contains("999999", error);
        Assert.Null(data);
    }

    // Escenario: NISE inválido
    [Fact]
    public async Task GuardarConfiguracion_NiseInvalido_RetornaError()
    {
        _actividadRepo.GetByCodigoAsync("551001").Returns(ActividadValida);
        var request = BuildRequest(nise: "123"); // menos de 10 dígitos

        var (success, error, data) = await _sut.GuardarConfiguracionAsync(request);

        Assert.False(success);
        Assert.Contains("NISE", error);
        Assert.Null(data);
    }

    // Escenario: Vinculación exitosa (nueva configuración)
    [Fact]
    public async Task GuardarConfiguracion_NuevaConfiguracion_VinculaTribuCrYGuarda()
    {
        _actividadRepo.GetByCodigoAsync("551001").Returns(ActividadValida);
        _configRepo.GetByAnfitrionIdAsync("anf-001").Returns((ConfiguracionTributaria?)null);

        var request = BuildRequest();

        var (success, error, data) = await _sut.GuardarConfiguracionAsync(request);

        Assert.True(success);
        Assert.Null(error);
        Assert.NotNull(data);
        Assert.StartsWith("TRIBU-", data!.TribuCr);
        Assert.Equal("551001", data.CodigoActividad);
        await _configRepo.Received(1).AddAsync(Arg.Any<ConfiguracionTributaria>());
    }

    // Escenario: Cambio de actividad económica muestra advertencia
    [Fact]
    public async Task GuardarConfiguracion_CambioActividad_MuestraAdvertencia()
    {
        var actividadNueva = new CatalogoActividadEconomica { Codigo = "682001", Descripcion = "Alquiler inmuebles", Vigente = true };
        _actividadRepo.GetByCodigoAsync("682001").Returns(actividadNueva);

        var existing = new ConfiguracionTributaria
        {
            Id = 1, AnfitrionId = "anf-001",
            CodigoActividad = "551001", ActividadEconomica = ActividadValida,
            TribuCr = "TRIBU-ANF-001-551001", DireccionFiscal = "San José", Nise = "1234567890"
        };
        _configRepo.GetByAnfitrionIdAsync("anf-001").Returns(existing);

        var request = BuildRequest(codigoActividad: "682001");

        var (success, _, data) = await _sut.GuardarConfiguracionAsync(request);

        Assert.True(success);
        Assert.NotNull(data!.Advertencia);
        Assert.Contains("actividad económica", data.Advertencia);
    }

    // Escenario: Cambio de actividad registra auditoría
    [Fact]
    public async Task GuardarConfiguracion_CambioActividad_RegistraAuditoria()
    {
        var actividadNueva = new CatalogoActividadEconomica { Codigo = "682001", Descripcion = "Alquiler inmuebles", Vigente = true };
        _actividadRepo.GetByCodigoAsync("682001").Returns(actividadNueva);

        var existing = new ConfiguracionTributaria
        {
            Id = 1, AnfitrionId = "anf-001",
            CodigoActividad = "551001", ActividadEconomica = ActividadValida,
            TribuCr = "TRIBU-ANF-001-551001", DireccionFiscal = "San José", Nise = "1234567890"
        };
        _configRepo.GetByAnfitrionIdAsync("anf-001").Returns(existing);

        await _sut.GuardarConfiguracionAsync(BuildRequest(codigoActividad: "682001"));

        await _configRepo.Received(1).AddAuditoriaAsync(Arg.Is<AuditoriaConfiguracion>(a =>
            a.Campo == "CAMBIO_ACTIVIDAD" &&
            a.ValorAnterior == "551001" &&
            a.ValorNuevo == "682001"));
    }

    // Escenario: Sin cambio de actividad no muestra advertencia
    [Fact]
    public async Task GuardarConfiguracion_SinCambioActividad_SinAdvertencia()
    {
        _actividadRepo.GetByCodigoAsync("551001").Returns(ActividadValida);

        var existing = new ConfiguracionTributaria
        {
            Id = 1, AnfitrionId = "anf-001",
            CodigoActividad = "551001", ActividadEconomica = ActividadValida,
            TribuCr = "TRIBU-ANF-001-551001", DireccionFiscal = "San José", Nise = "1234567890"
        };
        _configRepo.GetByAnfitrionIdAsync("anf-001").Returns(existing);

        var (_, _, data) = await _sut.GuardarConfiguracionAsync(BuildRequest());

        Assert.Null(data!.Advertencia);
    }

    // Escenario: Obtener catálogo de actividades económicas
    [Fact]
    public async Task GetActividades_RetornaListaActiva()
    {
        _actividadRepo.GetAllActivasAsync().Returns(new List<CatalogoActividadEconomica> { ActividadValida });

        var result = await _sut.GetActividadesAsync();

        Assert.Single(result);
        Assert.Equal("551001", result.First().Codigo);
    }

    private static ConfiguracionTributariaRequest BuildRequest(
        string anfitrionId = "anf-001",
        string codigoActividad = "551001",
        string direccion = "San José, Costa Rica",
        string nise = "1234567890") => new()
    {
        AnfitrionId = anfitrionId,
        CodigoActividad = codigoActividad,
        DireccionFiscal = direccion,
        Nise = nise
    };
}
