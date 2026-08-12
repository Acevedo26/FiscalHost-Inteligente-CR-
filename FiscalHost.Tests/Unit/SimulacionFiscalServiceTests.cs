using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Moq;
using Xunit;
using System.Text.Json;
using System.Linq;

namespace FiscalHost.Tests.Unit;

public class SimulacionFiscalServiceTests
{
    private readonly Mock<ISimulacionFiscalRepository> _mockRepo;
    private readonly SimulacionFiscalService _service;

    public SimulacionFiscalServiceTests()
    {
        _mockRepo = new Mock<ISimulacionFiscalRepository>();
        _service = new SimulacionFiscalService(_mockRepo.Object);
    }

    [Fact]
    public async Task CrearSimulacionAsync_ConValoresValidos_CreaYDevuelveSimulacion()
    {
        var usuarioId = Guid.NewGuid();
        var request = new CreateSimulacionFiscalRequest
        {
            Nombre = "Simulación Test",
            PeriodoBaseAnio = 2026,
            Parametros = new SimulacionParametrosDto
            {
                IngresosEstimados = 10000,
                GastosProyectados = 2000
            }
        };

        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<SimulacionFiscal>()))
                 .ReturnsAsync((SimulacionFiscal s) => s);

        var resultado = await _service.CrearSimulacionAsync(usuarioId, request);

        Assert.NotNull(resultado);
        Assert.Equal("Simulación Test", resultado.Nombre);
        Assert.Equal(1040, resultado.Resultados.IvaEstimado); // (10000 - 2000) * 0.13 = 1040
        Assert.Equal(1200, resultado.Resultados.RentaEstimada); // (10000 - 2000) * 0.15 = 1200
        Assert.Equal(560, resultado.Resultados.AhorroFiscalEsperado); // 2000 * 0.28 = 560
    }

    [Fact]
    public async Task CrearSimulacionAsync_ValoresNegativos_LanzaExcepcion()
    {
        var usuarioId = Guid.NewGuid();
        var request = new CreateSimulacionFiscalRequest
        {
            Nombre = "Simulación Error",
            Parametros = new SimulacionParametrosDto
            {
                IngresosEstimados = -1000,
                GastosProyectados = 500
            }
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CrearSimulacionAsync(usuarioId, request));
    }

    [Fact]
    public async Task CompararSimulacionesAsync_MasDeTresEscenarios_LanzaExcepcion()
    {
        var usuarioId = Guid.NewGuid();
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CompararSimulacionesAsync(usuarioId, ids));
    }

    [Fact]
    public async Task CompararSimulacionesAsync_IdsValidos_DevuelveComparacion()
    {
        var usuarioId = Guid.NewGuid();
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var ids = new List<Guid> { id1, id2 };

        var simulacion1 = new SimulacionFiscal
        {
            SimulacionId = id1,
            UsuarioId = usuarioId,
            Nombre = "S1"
        };
        var simulacion2 = new SimulacionFiscal
        {
            SimulacionId = id2,
            UsuarioId = usuarioId,
            Nombre = "S2"
        };

        _mockRepo.Setup(r => r.GetByIdAsync(id1, usuarioId)).ReturnsAsync(simulacion1);
        _mockRepo.Setup(r => r.GetByIdAsync(id2, usuarioId)).ReturnsAsync(simulacion2);

        var resultado = await _service.CompararSimulacionesAsync(usuarioId, ids);

        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Simulaciones.Count);
        Assert.Empty(resultado.Advertencias);
    }
}
