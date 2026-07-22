using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FiscalHost.Api.CR.Services;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Enums.Operations;

namespace FiscalHost.Tests.Unit;

public class GeneradorBorradorServiceTests
{
    private readonly Mock<IGeneradorBorradorRepository> _repositoryMock;
    private readonly GeneradorBorradorService _service;

    public GeneradorBorradorServiceTests()
    {
        _repositoryMock = new Mock<IGeneradorBorradorRepository>();
        _service = new GeneradorBorradorService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GenerarD104Async_ConRegistrosSinClasificar_LanzaExcepcion()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ContarReservasSinClasificarAsync(usuarioId, 2023, 10))
            .ReturnsAsync(2);
        _repositoryMock.Setup(r => r.ContarGastosPendientesAsync(usuarioId, 2023, 10))
            .ReturnsAsync(0);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GenerarD104Async(usuarioId, 2023, 10));
        Assert.Contains("No se puede generar el borrador porque hay 2 registros", ex.Message);
    }

    [Fact]
    public async Task GenerarD104Async_DatosValidos_CalculaIvaCorrectamente()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ContarReservasSinClasificarAsync(usuarioId, 2023, 10)).ReturnsAsync(0);
        _repositoryMock.Setup(r => r.ContarGastosPendientesAsync(usuarioId, 2023, 10)).ReturnsAsync(0);

        var reservas = new List<Reserva>
        {
            new Reserva { MontoBruto = 100000, ClasificacionFiscal = ClasificacionFiscal.GRAVADO },
            new Reserva { MontoBruto = 50000, ClasificacionFiscal = ClasificacionFiscal.EXENTO }
        };

        var gastos = new List<Gasto>
        {
            new Gasto { MontoIvaSoportado = 5000, EstadoValidacion = EstadoValidacion.VALIDO, EsCreditoFiscalValido = true },
            new Gasto { MontoIvaSoportado = 2000, EstadoValidacion = EstadoValidacion.PENDIENTE, EsCreditoFiscalValido = true }
        };

        _repositoryMock.Setup(r => r.ObtenerReservasAsync(usuarioId, 2023, 10)).ReturnsAsync(reservas);
        _repositoryMock.Setup(r => r.ObtenerGastosAsync(usuarioId, 2023, 10)).ReturnsAsync(gastos);

        // Act
        var result = await _service.GenerarD104Async(usuarioId, 2023, 10);

        // Assert
        Assert.Equal(100000, result.TotalIngresosGravados);
        Assert.Equal(13000, result.IvaCobrado); // 13% de 100000
        Assert.Equal(5000, result.IvaCreditoFiscal); // solo el valido
        Assert.Equal(8000, result.IvaNeto); // 13000 - 5000
        Assert.False(result.EsSaldoAFavor);
    }

    [Fact]
    public async Task GenerarD125Async_RegimenCapitalInmobiliario_CalculaBaseImponibleCorrectamente()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ContarReservasSinClasificarAsync(usuarioId, 2023, null)).ReturnsAsync(0);
        _repositoryMock.Setup(r => r.ContarGastosPendientesAsync(usuarioId, 2023, null)).ReturnsAsync(0);

        var reservas = new List<Reserva>
        {
            new Reserva { MontoBruto = 1000000, RetencionExtranjera = 20000 }
        };
        var gastos = new List<Gasto>(); // Gastos se ignoran en capital inmobiliario

        _repositoryMock.Setup(r => r.ObtenerReservasAsync(usuarioId, 2023, null)).ReturnsAsync(reservas);
        _repositoryMock.Setup(r => r.ObtenerGastosAsync(usuarioId, 2023, null)).ReturnsAsync(gastos);

        // Act
        var result = await _service.GenerarD125Async(usuarioId, 2023, regimenUtilidades: false);

        // Assert
        Assert.Equal(1000000, result.IngresoBrutoAnual);
        Assert.Equal(850000, result.BaseImponible); // 85% de 1M
        Assert.Equal(127500, result.ImpuestoRenta); // 15% de 850k
        Assert.Equal(20000, result.RetencionesExtranjeras);
        Assert.Equal(107500, result.ImpuestoNeto); // 127500 - 20000
    }

    [Fact]
    public async Task GenerarD125Async_RegimenUtilidades_CalculaBaseImponibleCorrectamente()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ContarReservasSinClasificarAsync(usuarioId, 2023, null)).ReturnsAsync(0);
        _repositoryMock.Setup(r => r.ContarGastosPendientesAsync(usuarioId, 2023, null)).ReturnsAsync(0);

        var reservas = new List<Reserva>
        {
            new Reserva { MontoBruto = 1000000, RetencionExtranjera = 10000 }
        };
        var gastos = new List<Gasto>
        {
            new Gasto { MontoTotal = 300000, EstadoValidacion = EstadoValidacion.VALIDO, EsDeducibleRenta = true },
            new Gasto { MontoTotal = 50000, EstadoValidacion = EstadoValidacion.VALIDO, EsDeducibleRenta = false }
        };

        _repositoryMock.Setup(r => r.ObtenerReservasAsync(usuarioId, 2023, null)).ReturnsAsync(reservas);
        _repositoryMock.Setup(r => r.ObtenerGastosAsync(usuarioId, 2023, null)).ReturnsAsync(gastos);

        // Act
        var result = await _service.GenerarD125Async(usuarioId, 2023, regimenUtilidades: true);

        // Assert
        Assert.Equal(700000, result.BaseImponible); // 1M - 300k
        Assert.Equal(105000, result.ImpuestoRenta); // 15% de 700k
        Assert.Equal(95000, result.ImpuestoNeto); // 105k - 10k
    }

    [Fact]
    public void GenerarD176_AtrasoMultaConTope_CalculaCorrectamente()
    {
        // Arrange
        decimal impuestoPrincipal = 5000000;
        var fechaVenc = new DateOnly(2023, 1, 1);
        var fechaAct = new DateOnly(2023, 8, 2); // > 7 meses

        // Act
        var result = _service.GenerarD176(impuestoPrincipal, fechaVenc, fechaAct);

        // Assert
        Assert.Equal(1386600m, result.MultaBase); // Llegó al tope
        Assert.Equal(1386600m * 0.20m, result.MultaReducida);
    }
}
