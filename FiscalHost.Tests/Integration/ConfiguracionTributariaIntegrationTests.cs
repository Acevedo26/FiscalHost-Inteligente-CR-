using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.DTOs;
using FiscalHost.Api.CR.Models.Entities;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Tests.Integration;

public class ConfiguracionTributariaIntegrationTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly ConfiguracionTributariaService _sut;

    private static readonly CatalogoActividadEconomica Actividad551001 = new()
    {
        Codigo = "551001", Descripcion = "Hoteles y alojamiento turístico", Vigente = true
    };

    private static readonly CatalogoActividadEconomica Actividad682001 = new()
    {
        Codigo = "682001", Descripcion = "Alquiler de inmuebles", Vigente = true
    };

    public ConfiguracionTributariaIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _db.CatalogoActividadesEconomicas.AddRange(Actividad551001, Actividad682001);
        _db.SaveChanges();

        var actividadRepo = new ActividadEconomicaRepository(_db);
        var configRepo = new ConfiguracionTributariaRepository(_db);
        _sut = new ConfiguracionTributariaService(configRepo, actividadRepo);
    }

    // Flujo completo: crear configuración y recuperarla
    [Fact]
    public async Task GuardarYObtener_ConfiguracionNueva_PersisteTodosLosCampos()
    {
        string anfitrionId = Guid.NewGuid().ToString();
        var (success, _, _) = await _sut.GuardarConfiguracionAsync(BuildRequest(anfitrionId: anfitrionId));
        var recuperada = await _sut.GetConfiguracionAsync(anfitrionId);

        Assert.True(success);
        Assert.NotNull(recuperada);
        Assert.Equal("551001", recuperada!.CodigoActividad);
        Assert.Equal("1234567890", recuperada.Nise);
        Assert.StartsWith("TRIBU-", recuperada.TribuCr);
    }

    // Cambio de actividad persiste auditoría en base de datos


    // Código inválido no persiste nada
    [Fact]
    public async Task CodigoInvalido_NoPersisteDatos()
    {
        string anfitrionId = Guid.NewGuid().ToString();
        var (success, _, _) = await _sut.GuardarConfiguracionAsync(BuildRequest(anfitrionId: anfitrionId, codigoActividad: "999999"));

        Assert.False(success);
        Assert.Empty(_db.PerfilesTributarios.Where(c => c.UsuarioId.ToString() == anfitrionId));
    }

    // NISE inválido no persiste nada
    [Fact]
    public async Task NiseInvalido_NoPersisteDatos()
    {
        string anfitrionId = Guid.NewGuid().ToString();
        var (success, _, _) = await _sut.GuardarConfiguracionAsync(BuildRequest(anfitrionId: anfitrionId, nise: "123"));

        Assert.False(success);
        Assert.Empty(_db.PerfilesTributarios.Where(c => c.UsuarioId.ToString() == anfitrionId));
    }

    // Actualización sin cambio de actividad no genera auditoría
    [Fact]
    public async Task ActualizacionSinCambioActividad_NoGeneraAuditoria()
    {
        string anfitrionId = Guid.NewGuid().ToString();
        await _sut.GuardarConfiguracionAsync(BuildRequest(anfitrionId: anfitrionId));
        await _sut.GuardarConfiguracionAsync(BuildRequest(anfitrionId: anfitrionId, direccion: "Cartago, Costa Rica"));

        var config = _db.PerfilesTributarios.First(c => c.UsuarioId.ToString() == anfitrionId);
        Assert.Empty(_db.AuditoriasConfiguracion.Where(a =>
            a.ConfiguracionTributariaId == config.PerfilId.GetHashCode() && a.Campo == "CAMBIO_ACTIVIDAD"));
    }

    public void Dispose() => _db.Dispose();

    private static ConfiguracionTributariaRequest BuildRequest(
        string? anfitrionId = null,
        string codigoActividad = "551001",
        string direccion = "San José, Costa Rica",
        string nise = "1234567890") => new()
    {
        AnfitrionId = anfitrionId ?? Guid.NewGuid().ToString(),
        CodigoActividad = codigoActividad,
        DireccionFiscal = direccion,
        Nise = nise
    };
}
