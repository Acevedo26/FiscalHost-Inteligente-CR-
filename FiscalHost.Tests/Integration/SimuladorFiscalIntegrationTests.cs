using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Controllers;
using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FiscalHost.Tests.Integration;

public class SimuladorFiscalIntegrationTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly SimulacionFiscalRepository _repository;
    private readonly SimulacionFiscalService _service;
    private readonly SimuladorFiscalController _controller;

    public SimuladorFiscalIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();

        _repository = new SimulacionFiscalRepository(_db);
        _service = new SimulacionFiscalService(_repository);
        _controller = new SimuladorFiscalController(_service);
    }

    [Fact]
    public async Task CrearSimulacion_RetornaSimulacionCorrecta()
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

        var request = new CreateSimulacionFiscalRequest
        {
            Nombre = "Prueba Integracion",
            PeriodoBaseAnio = 2026,
            Parametros = new SimulacionParametrosDto
            {
                IngresosEstimados = 5000,
                GastosProyectados = 1000
            }
        };

        var result = await _controller.CrearSimulacion(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<SimulacionFiscalResponseDto>(createdResult.Value);

        Assert.Equal("Prueba Integracion", dto.Nombre);
        Assert.Equal(520, dto.Resultados.IvaEstimado); // (5000 - 1000) * 0.13 = 520
        Assert.Equal(600, dto.Resultados.RentaEstimada); // (5000 - 1000) * 0.15 = 600
    }

    public void Dispose() => _db.Dispose();
}
