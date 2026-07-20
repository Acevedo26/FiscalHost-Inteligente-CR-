using System;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums.Operations;
using FiscalHost.Api.CR.Models.Enums.TaxIntelligence;
using FiscalHost.Api.CR.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FiscalHost.Tests.Integration;

public class CalculoIvaServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly CalculoIvaService _sut;

    public CalculoIvaServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _sut = new CalculoIvaService(_db);
    }

    [Fact]
    public async Task CalcularIvaDevengadoAsync_Exitoso()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        short anio = 2026;
        short mes = 7;

        await _db.PeriodosFiscales.AddAsync(new PeriodoFiscal
        {
            PeriodoId = Guid.NewGuid(),
            Anio = anio,
            Mes = mes
        });

        await _db.Reservas.AddAsync(new Reserva
        {
            ReservaId = Guid.NewGuid(),
            UsuarioId = usuarioId,
            PeriodoFiscalAnio = anio,
            PeriodoFiscalMes = mes,
            Estado = "COMPLETADO",
            MontoColones = 113000,
            MontoGravado = 100000,
            MontoExento = 0,
            MontoIvaCalculado = 13000
        });

        await _db.Gastos.AddAsync(new Gasto
        {
            GastoId = Guid.NewGuid(),
            UsuarioId = usuarioId,
            PeriodoFiscalAnio = anio,
            PeriodoFiscalMes = mes,
            EstadoValidacion = EstadoValidacion.VALIDO,
            EsCreditoFiscalValido = true,
            MontoIvaSoportado = 5000
        });

        await _db.SaveChangesAsync();

        // Act
        var result = await _sut.CalcularIvaDevengadoAsync(usuarioId, anio, mes);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(13000, result.DebitoFiscal);
        Assert.Equal(5000, result.CreditoFiscal);
        Assert.Equal(8000, result.IvaNeto);
        Assert.Equal(8000, result.MontoTotalAPagar);
        Assert.Equal(0, result.SaldoFavorResultante);
    }

    [Fact]
    public async Task CalcularIvaDevengadoAsync_SaldoAFavor()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        short anio = 2026;
        short mes = 7;

        await _db.PeriodosFiscales.AddAsync(new PeriodoFiscal
        {
            PeriodoId = Guid.NewGuid(),
            Anio = anio,
            Mes = mes
        });

        await _db.Reservas.AddAsync(new Reserva
        {
            ReservaId = Guid.NewGuid(),
            UsuarioId = usuarioId,
            PeriodoFiscalAnio = anio,
            PeriodoFiscalMes = mes,
            Estado = "COMPLETADO",
            MontoIvaCalculado = 13000
        });

        await _db.Gastos.AddAsync(new Gasto
        {
            GastoId = Guid.NewGuid(),
            UsuarioId = usuarioId,
            PeriodoFiscalAnio = anio,
            PeriodoFiscalMes = mes,
            EstadoValidacion = EstadoValidacion.VALIDO,
            EsCreditoFiscalValido = true,
            MontoIvaSoportado = 20000 // Mayor al débito
        });

        await _db.SaveChangesAsync();

        // Act
        var result = await _sut.CalcularIvaDevengadoAsync(usuarioId, anio, mes);

        // Assert
        Assert.Equal(13000, result.DebitoFiscal);
        Assert.Equal(20000, result.CreditoFiscal);
        Assert.Equal(0, result.IvaNeto); // No se paga
        Assert.Equal(7000, result.SaldoFavorResultante);
    }

    [Fact]
    public async Task CalcularIvaDevengadoAsync_DatosIncompletos_LanzaExcepcion()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        short anio = 2026;
        short mes = 7;

        await _db.Reservas.AddAsync(new Reserva
        {
            ReservaId = Guid.NewGuid(),
            UsuarioId = usuarioId,
            PeriodoFiscalAnio = anio,
            PeriodoFiscalMes = mes,
            Estado = "EN_REVISION"
        });

        await _db.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _sut.CalcularIvaDevengadoAsync(usuarioId, anio, mes));
    }

    public void Dispose()
    {
        _db.Database.EnsureDeleted();
        _db.Dispose();
    }
}
