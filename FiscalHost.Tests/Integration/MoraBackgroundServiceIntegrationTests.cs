using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using FiscalHost.Api.CR.Models;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums.TaxIntelligence;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using System.Collections.Generic;

namespace FiscalHost.Tests.Integration;

public class MoraBackgroundServiceIntegrationTests
{
    [Fact]
    public async Task MoraBackgroundService_DeberiaLlamarProcesarCargosMoratorios()
    {
        // Arrange
        var repoMock = new Mock<IObligacionTributariaRepository>();
        
        var pendientes = new List<ObligacionTributaria>
        {
            new ObligacionTributaria { 
                ObligacionId = Guid.NewGuid(), 
                UsuarioId = Guid.NewGuid(),
                MontoCapital = 100000, 
                Estado = EstadoObligacion.VIGENTE,
                FechaVencimiento = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2))
            }
        };

        repoMock.Setup(r => r.GetVencidasPendientesAsync(It.IsAny<DateOnly>()))
                .ReturnsAsync(pendientes);

        var services = new ServiceCollection();
        services.AddSingleton(repoMock.Object);
        services.AddScoped<IObligacionTributariaService, ObligacionTributariaService>();
        services.AddSingleton(Options.Create(new TaxSettings { DefaultInterestRate = 0.115m }));
        services.AddSingleton(new Mock<INotificacionService>().Object);
        services.AddSingleton(new Mock<ILogger<ObligacionTributariaService>>().Object);

        var serviceProvider = services.BuildServiceProvider();
        
        // Obtenemos el servicio directamente para probar el método (saltando el Task.Delay del loop)
        var obligacionService = serviceProvider.GetRequiredService<IObligacionTributariaService>();
        
        // Act
        await obligacionService.ProcesarCargosMoratoriosMasivosAsync(DateOnly.FromDateTime(DateTime.UtcNow));

        // Assert
        repoMock.Verify(r => r.UpdateRangeAsync(It.IsAny<IEnumerable<ObligacionTributaria>>()), Times.Once);
        repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.True(pendientes[0].MontoInteresesAcumulados > 0);
    }
}
