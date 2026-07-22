using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Enums.Operations;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Tests.Integration;

public class ExportacionHaciendaIntegrationTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly ExportacionHaciendaService _sut;

    public ExportacionHaciendaIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _sut = new ExportacionHaciendaService(new ExportacionHaciendaRepository(_db));
    }

    [Fact]
    public async Task Exportar_ConDatos_RegistraExportacion()
    {
        var usuarioId = Guid.NewGuid();
        _db.Reservas.Add(new Reserva
        {
            ReservaId = Guid.NewGuid(),
            UsuarioId = usuarioId,
            FechaInicio = new DateTime(2026, 2, 1),
            FechaFin = new DateTime(2026, 2, 2),
            PeriodoFiscalAnio = 2026,
            PeriodoFiscalMes = 2,
            MontoBruto = 1000,
            MontoColones = 1000,
            MontoGravado = 1000,
            PlataformaOrigen = PlataformaOrigen.DIRECTA
        });
        await _db.SaveChangesAsync();

        var result = await _sut.ExportarAsync(new ExportacionHaciendaRequest
        {
            UsuarioId = usuarioId,
            AnioFiscal = 2026,
            Mes = 2,
            Formato = "CSV",
            TipoContenido = "MOVIMIENTOS"
        });

        Assert.True(result.Success);
        Assert.Single(_db.Exportaciones);
        Assert.Equal("CSV", _db.Exportaciones.Single().Formato);
    }

    public void Dispose() => _db.Dispose();
}
