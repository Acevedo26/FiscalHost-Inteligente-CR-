using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums.Operations;
using FiscalHost.Api.CR.Models.Enums.TaxIntelligence;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Tests.Integration;

public class ReconstruccionBaseImponibleIntegrationTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly ReconstruccionBaseImponibleService _sut;

    public ReconstruccionBaseImponibleIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _sut = new ReconstruccionBaseImponibleService(
            new ReconstruccionBaseImponibleRepository(_db));
    }

    [Fact]
    public async Task Reconstruir_ConContinuacion_GuardaCalculoFiscal()
    {
        var usuarioId = Guid.NewGuid();
        _db.Reservas.Add(new Reserva
        {
            ReservaId = Guid.NewGuid(),
            UsuarioId = usuarioId,
            PeriodoFiscalAnio = 2025,
            PeriodoFiscalMes = 1,
            MontoBruto = 1000,
            MontoColones = 1000,
            MontoGravado = 1000,
            ClasificacionFiscal = ClasificacionFiscal.GRAVADO
        });
        _db.PeriodosFiscales.Add(new PeriodoFiscal
        {
            PeriodoId = Guid.NewGuid(),
            Anio = 2025,
            Mes = 1,
            TipoFormulario = TipoFormulario.D104,
            TarifaIva = 0.13m,
            TarifaRentaCapital = 0.15m,
            DeduccionPlanaCapital = 0.85m
        });
        await _db.SaveChangesAsync();

        var result = await _sut.ReconstruirAsync(new ReconstruccionBaseImponibleRequest
        {
            UsuarioId = usuarioId,
            AnioFiscal = 2025,
            ContinuarConDatosIncompletos = true
        });

        Assert.True(result.Success);
        Assert.Single(_db.CalculosFiscales);
        Assert.Equal(257.50m, _db.CalculosFiscales.Single().MontoTotalAPagar);
    }

    public void Dispose() => _db.Dispose();
}
