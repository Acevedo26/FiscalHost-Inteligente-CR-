using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.DTOs.Operations.Requests;
using FiscalHost.Api.CR.Models.Enums.Operations;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Tests.Integration;

public class ClasificacionIngresoIntegrationTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly ClasificacionIngresoService _sut;

    public ClasificacionIngresoIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        var repo = new ClasificacionIngresoRepository(_db);
        _sut = new ClasificacionIngresoService(repo);
    }

    [Fact]
    public async Task ClasificarYPersistir_IngresoValido_GuardaClasificacion()
    {
        var (success, _, data) = await _sut.ClasificarAsync(BuildRequest());

        Assert.True(success);
        Assert.NotNull(data);
        Assert.Single(_db.ClasificacionesIngresos);
        Assert.Equal("Gravado 13% IVA", data!.ClasificacionIva);
    }

    [Fact]
    public async Task Reclasificar_ConJustificacion_GuardaAuditoria()
    {
        var (_, _, data) = await _sut.ClasificarAsync(BuildRequest());

        await _sut.ReclasificarAsync(data!.Id, new ReclasificacionIngresoRequest
        {
            UsuarioId = "usr-001",
            ClasificacionIva = ClasificacionIva.Exento,
            Justificacion = "Ajuste validado por documento de respaldo."
        });

        var auditoria = _db.AuditoriasClasificacionIngresos.Single();
        Assert.Equal("Gravado13", auditoria.ValorAnterior);
        Assert.Equal("Exento", auditoria.ValorNuevo);
        Assert.Contains("documento", auditoria.Justificacion);
    }

    public void Dispose() => _db.Dispose();

    private static ClasificarIngresoRequest BuildRequest() => new()
    {
        AnfitrionId = "anf-integ-006",
        FechaEntrada = DateTime.UtcNow.AddDays(-7),
        FechaSalida = DateTime.UtcNow,
        MontoBruto = 1000,
        FuenteIngreso = FuenteIngreso.Nacional,
        HuespedResidente = true
    };
}
