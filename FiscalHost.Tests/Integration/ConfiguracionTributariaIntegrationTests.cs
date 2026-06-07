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

    private static readonly ActividadEconomica Actividad551001 = new()
    {
        Id = 1, Codigo = "551001", Descripcion = "Hoteles y alojamiento turístico", Activa = true
    };

    private static readonly ActividadEconomica Actividad682001 = new()
    {
        Id = 2, Codigo = "682001", Descripcion = "Alquiler de inmuebles", Activa = true
    };

    public ConfiguracionTributariaIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _db.ActividadesEconomicas.AddRange(Actividad551001, Actividad682001);
        _db.SaveChanges();

        var actividadRepo = new ActividadEconomicaRepository(_db);
        var configRepo = new ConfiguracionTributariaRepository(_db);
        _sut = new ConfiguracionTributariaService(configRepo, actividadRepo);
    }

    // Flujo completo: crear configuración y recuperarla
    [Fact]
    public async Task GuardarYObtener_ConfiguracionNueva_PersisteTodosLosCampos()
    {
        const string anfitrionId = "anf-integ-001";
        var (success, _, _) = await _sut.GuardarConfiguracionAsync(BuildRequest());
        var recuperada = await _sut.GetConfiguracionAsync(anfitrionId);

        Assert.True(success);
        Assert.NotNull(recuperada);
        Assert.Equal("551001", recuperada!.CodigoActividad);
        Assert.Equal("1234567890", recuperada.Nise);
        Assert.StartsWith("TRIBU-", recuperada.TribuCr);
    }

    // Cambio de actividad persiste auditoría en base de datos
    [Fact]
    public async Task CambioActividad_RegistraAuditoriaEnBd()
    {
        await _sut.GuardarConfiguracionAsync(BuildRequest(codigoActividad: "551001"));
        await _sut.GuardarConfiguracionAsync(BuildRequest(codigoActividad: "682001"));

        var auditoria = _db.AuditoriasConfiguracion
            .FirstOrDefault(a => a.Campo == "CAMBIO_ACTIVIDAD");

        Assert.NotNull(auditoria);
        Assert.Equal("551001", auditoria!.ValorAnterior);
        Assert.Equal("682001", auditoria.ValorNuevo);
    }

    // Código inválido no persiste nada
    [Fact]
    public async Task CodigoInvalido_NoPersisteDatos()
    {
        const string anfitrionId = "anf-codigo-invalido";
        var (success, _, _) = await _sut.GuardarConfiguracionAsync(BuildRequest(anfitrionId: anfitrionId, codigoActividad: "999999"));

        Assert.False(success);
        Assert.Empty(_db.ConfiguracionesTributarias.Where(c => c.AnfitrionId == anfitrionId));
    }

    // NISE inválido no persiste nada
    [Fact]
    public async Task NiseInvalido_NoPersisteDatos()
    {
        const string anfitrionId = "anf-nise-invalido";
        var (success, _, _) = await _sut.GuardarConfiguracionAsync(BuildRequest(anfitrionId: anfitrionId, nise: "123"));

        Assert.False(success);
        Assert.Empty(_db.ConfiguracionesTributarias.Where(c => c.AnfitrionId == anfitrionId));
    }

    // Actualización sin cambio de actividad no genera auditoría
    [Fact]
    public async Task ActualizacionSinCambioActividad_NoGeneraAuditoria()
    {
        const string anfitrionId = "anf-sin-cambio";
        await _sut.GuardarConfiguracionAsync(BuildRequest(anfitrionId: anfitrionId));
        await _sut.GuardarConfiguracionAsync(BuildRequest(anfitrionId: anfitrionId, direccion: "Cartago, Costa Rica"));

        var config = _db.ConfiguracionesTributarias.First(c => c.AnfitrionId == anfitrionId);
        Assert.Empty(_db.AuditoriasConfiguracion.Where(a =>
            a.ConfiguracionTributariaId == config.Id && a.Campo == "CAMBIO_ACTIVIDAD"));
    }

    public void Dispose() => _db.Dispose();

    private static ConfiguracionTributariaRequest BuildRequest(
        string anfitrionId = "anf-integ-001",
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
