using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FiscalHost.Api.CR.Models;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;

namespace FiscalHost.Tests.Unit;

public class ObligacionTributariaServiceTests
{
    private readonly Mock<IObligacionTributariaRepository> _repoMock;
    private readonly Mock<INotificacionService> _notificacionMock;
    private readonly Mock<ILogger<ObligacionTributariaService>> _loggerMock;
    private readonly IOptions<TaxSettings> _settings;

    public ObligacionTributariaServiceTests()
    {
        _repoMock = new Mock<IObligacionTributariaRepository>();
        _notificacionMock = new Mock<INotificacionService>();
        _loggerMock = new Mock<ILogger<ObligacionTributariaService>>();
        _settings = Options.Create(new TaxSettings { DefaultInterestRate = 0.115m }); // 11.5% anual
    }

    [Fact]
    public async Task CalcularIntereses_DeberiaCalcularInteresSimple_ConAnoBisiesto()
    {
        // Arrange
        var service = new ObligacionTributariaService(_repoMock.Object, _settings, _notificacionMock.Object, _loggerMock.Object);
        var obligacion = new ObligacionTributaria
        {
            ObligacionId = Guid.NewGuid(),
            UsuarioId = Guid.NewGuid(),
            MontoCapital = 100000, // 100,000
            MontoMulta = 0,
            FechaVencimiento = new DateOnly(2024, 1, 1),
            HistorialIntereses = "{}"
        };

        // Año 2024 es bisiesto, 366 días.
        // Tasa diaria = 0.115 / 366 = 0.0003142
        // Interés = 100000 * 0.0003142 = 31.42 (aprox 31.42 redondeado a 2 decimales)
        var fechaActual = new DateOnly(2024, 1, 2);

        // Act
        await service.CalcularInteresesObligacionAsync(obligacion, fechaActual, notificarCambios: false);

        // Assert
        Assert.Equal(31.42m, obligacion.MontoInteresesAcumulados);
        Assert.Equal(100031.42m, obligacion.MontoTotalActualizado);
        Assert.Contains("2024-01-02", obligacion.HistorialIntereses);
    }

    [Fact]
    public async Task CalcularIntereses_NoDebeCalcularSiFechaVencimientoEsDefault()
    {
        // Arrange
        var service = new ObligacionTributariaService(_repoMock.Object, _settings, _notificacionMock.Object, _loggerMock.Object);
        var obligacion = new ObligacionTributaria
        {
            ObligacionId = Guid.NewGuid(),
            MontoCapital = 100000,
            FechaVencimiento = default
        };

        var fechaActual = new DateOnly(2024, 1, 2);

        // Act
        await service.CalcularInteresesObligacionAsync(obligacion, fechaActual, notificarCambios: false);

        // Assert
        Assert.Equal(0, obligacion.MontoInteresesAcumulados);
        Assert.Equal(0, obligacion.MontoTotalActualizado); // Nunca se procesó
    }
}
