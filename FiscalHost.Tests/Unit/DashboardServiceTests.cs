using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Services;
using FiscalHost.Api.CR.Models.Entities.Operations;

namespace FiscalHost.Tests.Unit;

public class DashboardServiceTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task GetDashboardDataAsync_NoData_ReturnsAltoRiesgo()
    {
        var context = GetDbContext();
        var service = new DashboardService(context);
        var usuarioId = Guid.NewGuid();

        var result = await service.GetDashboardDataAsync(usuarioId, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);

        Assert.False(result.TieneDatos);
        Assert.Equal("Alto", result.RiesgoFiscal.NivelRiesgo);
        Assert.Contains(result.RiesgoFiscal.Factores, f => f.Contains("No hay datos"));
    }

    [Fact]
    public async Task GetDashboardDataAsync_OnlyIngresos_ReturnsAltoRiesgo()
    {
        var context = GetDbContext();
        var usuarioId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        context.Reservas.Add(new Reserva
        {
            ReservaId = Guid.NewGuid(),
            UsuarioId = usuarioId,
            FechaInicio = now,
            FechaFin = now.AddDays(2),
            MontoColones = 1000,
            MontoIvaCalculado = 130,
            Estado = "Completada"
        });
        await context.SaveChangesAsync();

        var service = new DashboardService(context);

        var result = await service.GetDashboardDataAsync(usuarioId, now.AddDays(-1), now.AddDays(1));

        Assert.True(result.TieneDatos);
        Assert.Equal("Alto", result.RiesgoFiscal.NivelRiesgo);
        Assert.Equal(1000, result.Metricas.IngresosBrutos);
        Assert.Contains(result.RiesgoFiscal.Factores, f => f.Contains("ningún gasto"));
    }

    [Fact]
    public async Task GetDashboardDataAsync_Balanced_ReturnsBajoRiesgo()
    {
        var context = GetDbContext();
        var usuarioId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        context.Reservas.Add(new Reserva
        {
            ReservaId = Guid.NewGuid(),
            UsuarioId = usuarioId,
            FechaInicio = now,
            FechaFin = now.AddDays(2),
            MontoColones = 1000,
            MontoIvaCalculado = 130,
            Estado = "Completada"
        });

        context.Gastos.Add(new Gasto
        {
            GastoId = Guid.NewGuid(),
            UsuarioId = usuarioId,
            Proveedor = "Test",
            FechaEmision = DateOnly.FromDateTime(now),
            MontoColones = 500,
            MontoIvaSoportado = 65,
            EsCreditoFiscalValido = true,
            EsDeducibleRenta = true
        });

        await context.SaveChangesAsync();

        var service = new DashboardService(context);

        var result = await service.GetDashboardDataAsync(usuarioId, now.AddDays(-1), now.AddDays(1));

        Assert.True(result.TieneDatos);
        Assert.Equal("Bajo", result.RiesgoFiscal.NivelRiesgo);
        
        Assert.Equal(1000, result.Metricas.IngresosBrutos);
        Assert.Equal(140, result.Metricas.ImpuestosEstimados);
        Assert.Equal(1000 - 140 - 500, result.Metricas.IngresosNetos);

        Assert.Single(result.EvolucionMensual);
        Assert.Equal(1000, result.EvolucionMensual[0].IngresosBrutos);
    }
}
