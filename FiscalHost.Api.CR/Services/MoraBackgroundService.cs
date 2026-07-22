using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FiscalHost.Api.CR.Services;

public class MoraBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<MoraBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("MoraBackgroundService está iniciando.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;
                // Calculamos cuánto falta para la próxima medianoche en hora local (Costa Rica es UTC-6 aprox)
                // Para simplificar, configuramos para correr a las 12:00 AM (UTC)
                var nextMidnight = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1);
                var delay = nextMidnight - now;

                logger.LogInformation("MoraBackgroundService esperará {Delay} hasta la próxima ejecución.", delay);

                await Task.Delay(delay, stoppingToken);

                logger.LogInformation("MoraBackgroundService iniciando cálculo masivo de mora...");

                using (var scope = serviceProvider.CreateScope())
                {
                    var obligacionService = scope.ServiceProvider.GetRequiredService<IObligacionTributariaService>();
                    var fechaCorte = DateOnly.FromDateTime(DateTime.UtcNow);
                    
                    await obligacionService.ProcesarCargosMoratoriosMasivosAsync(fechaCorte);
                }
            }
            catch (TaskCanceledException)
            {
                // Ignorar al detener el servicio
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocurrió un error ejecutando el cálculo de mora.");
                // Retraso de seguridad antes de reintentar si hay error
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}
