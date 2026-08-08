using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Services;
using FiscalHost.Api.CR.Controllers;
using FiscalHost.Api.CR.Models.DTOs.Dashboard;
using FiscalHost.Api.CR.Models.Entities.Operations;
using System.Security.Claims;

namespace FiscalHost.Tests.Integration;

public class DashboardIntegrationTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly DashboardService _service;
    private readonly DashboardController _controller;

    public DashboardIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
        
        _service = new DashboardService(_db);
        _controller = new DashboardController(_service);
    }

    [Fact]
    public async Task GetDashboardData_Success()
    {
        var usuarioId = Guid.NewGuid();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString())
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var now = DateTime.UtcNow;

        _db.Reservas.Add(new Reserva
        {
            ReservaId = Guid.NewGuid(),
            UsuarioId = usuarioId,
            FechaInicio = now,
            FechaFin = now.AddDays(1),
            MontoColones = 2000,
            MontoIvaCalculado = 260,
            Estado = "Completada"
        });

        await _db.SaveChangesAsync();

        var result = await _controller.GetDashboardData(now.AddDays(-1), now.AddDays(1));

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<DashboardResponseDto>(okResult.Value);

        Assert.True(dto.TieneDatos);
        Assert.Equal(2000, dto.Metricas.IngresosBrutos);
    }

    public void Dispose() => _db.Dispose();
}
