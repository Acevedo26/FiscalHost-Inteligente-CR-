using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.DTOs.Audit.Requests;
using FiscalHost.Api.CR.Models.Entities.Audit;
using FiscalHost.Api.CR.Models.Enums.Audit;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Tests.Integration;

public class AuditoriaInalterableIntegrationTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly AuditoriaInalterableService _sut;

    public AuditoriaInalterableIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _sut = new AuditoriaInalterableService(
            new AuditoriaInalterableRepository(_db));
    }

    [Fact]
    public async Task RegistrarYConsultarHistorial_RetornaOrdenCronologico()
    {
        var registroId = Guid.NewGuid();

        await _sut.RegistrarAsync(BuildRequest(registroId, "valor-1"));
        await _sut.RegistrarAsync(BuildRequest(registroId, "valor-2"));

        var historial = await _sut.ConsultarHistorialAsync(null, "usuario", registroId);

        Assert.Equal(2, historial.Count);
        Assert.True(historial[0].CreatedAt <= historial[1].CreatedAt);
    }

    [Fact]
    public async Task ExportarHistorial_ConDatos_GeneraCsvBase64()
    {
        var registroId = Guid.NewGuid();
        await _sut.RegistrarAsync(BuildRequest(registroId, "valor"));

        var result = await _sut.ExportarHistorialAsync(null, "usuario", registroId);

        Assert.True(result.Success);
        Assert.EndsWith(".csv", result.NombreArchivo);
        Assert.NotEmpty(result.ContenidoBase64);
    }

    [Fact]
    public async Task ModificarAuditoria_GeneraErrorPorInmutabilidad()
    {
        var auditoria = new AuditoriaOperacion
        {
            AuditId = Guid.NewGuid(),
            Operacion = OperacionAuditoria.UPDATE,
            TablaAfectada = "usuario"
        };
        _db.AuditoriasOperacion.Add(auditoria);
        await _db.SaveChangesAsync();

        auditoria.TablaAfectada = "tabla_modificada";

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _db.SaveChangesAsync());
    }

    [Fact]
    public async Task EliminarAuditoria_GeneraErrorPorInmutabilidad()
    {
        var auditoria = new AuditoriaOperacion
        {
            AuditId = Guid.NewGuid(),
            Operacion = OperacionAuditoria.UPDATE,
            TablaAfectada = "usuario"
        };
        _db.AuditoriasOperacion.Add(auditoria);
        await _db.SaveChangesAsync();

        _db.AuditoriasOperacion.Remove(auditoria);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _db.SaveChangesAsync());
    }

    public void Dispose() => _db.Dispose();

    private static RegistrarAuditoriaRequest BuildRequest(Guid registroId, string valor) => new()
    {
        Operacion = OperacionAuditoria.UPDATE,
        TablaAfectada = "usuario",
        RegistroId = registroId,
        OldValues = "{}",
        NewValues = $"{{\"campo\":\"{valor}\"}}",
        CamposModificados = ["campo"],
        Justificacion = "Prueba de auditoria."
    };
}
