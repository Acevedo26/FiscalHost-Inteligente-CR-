using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FiscalHost.Api.CR.Services;

public class AlertaBackgroundService(
	IServiceProvider serviceProvider,
	ILogger<AlertaBackgroundService> logger) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("AlertaBackgroundService está iniciando.");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				var now = DateTime.UtcNow;
				var nextRun = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0, DateTimeKind.Utc);
				if (nextRun <= now)
				{
					nextRun = nextRun.AddDays(1);
				}

				var delay = nextRun - now;
				logger.LogInformation("AlertaBackgroundService esperará {Delay} hasta la próxima ejecución.", delay);
				await Task.Delay(delay, stoppingToken);

				using (var scope = serviceProvider.CreateScope())
				{
					var alertaService = scope.ServiceProvider.GetRequiredService<IAlertaService>();
					var fechaActual = DateOnly.FromDateTime(DateTime.UtcNow);

					await alertaService.GenerarAlertasVencimientoAsync(fechaActual);
					await alertaService.EnviarAlertasPendientesAsync(DateTimeOffset.UtcNow);
				}
			}
			catch (TaskCanceledException)
			{
				// Ignorar al detener el servicio
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Ocurrió un error ejecutando la generación/envío de alertas.");
				await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
			}
		}
	}
}