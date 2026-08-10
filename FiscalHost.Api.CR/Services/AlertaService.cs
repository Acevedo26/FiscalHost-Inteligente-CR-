using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FiscalHost.Api.CR.Models.Entities.Communication;
using FiscalHost.Api.CR.Models.Enums.Communication;
using FiscalHost.Api.CR.Models.DTOs.Communication.Responses;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public class AlertaService(
	IAlertaRepository alertaRepository,
	IObligacionTributariaRepository obligacionRepository,
	INotificacionService notificacionService,
	ILogger<AlertaService> logger) : IAlertaService
{
	// RF-013: Generación de alertas proactivas 15, 10, 7, 3 y 1 día antes del vencimiento.
	// Umbral -> (TipoAlerta, Prioridad). Prioridad más baja = más urgente.
	private static readonly Dictionary<int, (TipoAlerta Tipo, short Prioridad)> UmbralesAlerta = new()
	{
		{ 15, (TipoAlerta.VENCIMIENTO_15_DIAS, 5) },
		{ 10, (TipoAlerta.VENCIMIENTO_10_DIAS, 4) },
		{ 7, (TipoAlerta.VENCIMIENTO_7_DIAS, 3) },
		{ 3, (TipoAlerta.VENCIMIENTO_3_DIAS, 2) },
		{ 1, (TipoAlerta.VENCIMIENTO_1_DIA, 1) },
	};

	public async Task GenerarAlertasVencimientoAsync(DateOnly fechaActual)
	{
		var proximasAVencer = await obligacionRepository.GetProximasAVencerAsync(fechaActual, diasMaximo: 15);
		var nuevasAlertas = new List<Alerta>();

		foreach (var obligacion in proximasAVencer)
		{
			var diasRestantes = obligacion.FechaVencimiento.DayNumber - fechaActual.DayNumber;

			if (!UmbralesAlerta.TryGetValue(diasRestantes, out var umbral))
			{
				continue;
			}

			var yaExiste = await alertaRepository.ExisteAlertaParaObligacionAsync(obligacion.ObligacionId, umbral.Tipo);
			if (yaExiste)
			{
				continue;
			}

			var esUltimoDia = umbral.Tipo == TipoAlerta.VENCIMIENTO_1_DIA;

			nuevasAlertas.Add(new Alerta
			{
				AlertaId = Guid.NewGuid(),
				UsuarioId = obligacion.UsuarioId,
				ObligacionId = obligacion.ObligacionId,
				TipoAlerta = umbral.Tipo,
				Titulo = esUltimoDia
					? $"¡Urgente! Vencimiento mañana: {obligacion.TipoFormulario}"
					: $"Vencimiento en {diasRestantes} día(s): {obligacion.TipoFormulario}",
				Mensaje = esUltimoDia
					? $"Recordatorio urgente: la obligación '{obligacion.Descripcion}' vence mañana ({obligacion.FechaVencimiento:dd/MM/yyyy}). " +
					  $"Monto actualizado: ₡{obligacion.MontoTotalActualizado}. Genere su borrador ahora para evitar sanciones."
					: $"La obligación '{obligacion.Descripcion}' vence el {obligacion.FechaVencimiento:dd/MM/yyyy}. " +
					  $"Monto actualizado: ₡{obligacion.MontoTotalActualizado}. Evite recargos declarando a tiempo.",
				Prioridad = umbral.Prioridad,
				MontoEstimado = obligacion.MontoTotalActualizado,
				Canal = UsuarioService.ResolverCanalPreferido(obligacion.Usuario?.PreferenciasNotificacion ?? string.Empty),
				Estado = EstadoNotificacion.PENDIENTE,
				AccionSugerida = $"{{\"accion\":\"generar-borrador\",\"obligacionId\":\"{obligacion.ObligacionId}\"}}",
				FechaProgramada = DateTimeOffset.UtcNow,
				IntentosEnvio = 0,
			});
		}

		if (nuevasAlertas.Count == 0)
		{
			logger.LogInformation("No se generaron nuevas alertas de vencimiento para la fecha {Fecha}.", fechaActual);
			return;
		}

		await alertaRepository.AddRangeAsync(nuevasAlertas);
		await alertaRepository.SaveChangesAsync();
		logger.LogInformation("Se generaron {Cantidad} alertas de vencimiento para la fecha {Fecha}.", nuevasAlertas.Count, fechaActual);
	}

	public async Task EnviarAlertasPendientesAsync(DateTimeOffset fechaCorte)
	{
		var pendientes = await alertaRepository.GetPendientesParaEnvioAsync(fechaCorte);

		foreach (var alerta in pendientes)
		{
			try
			{
				if (alerta.Canal != CanalNotificacion.PLATAFORMA)
				{
					// CORREO o AMBOS: se despacha por el canal de notificación externo.
					// Si el usuario eligió solo PLATAFORMA, la alerta ya quedó visible
					// dentro del sistema (persistida) y no se envía correo.
					await notificacionService.NotificarAsync(alerta.UsuarioId.ToString(), alerta.Mensaje);
				}

				alerta.Estado = EstadoNotificacion.ENVIADA;
				alerta.FechaEnvio = DateTimeOffset.UtcNow;
				alerta.IntentosEnvio++;
				alerta.ErrorEnvio = null;
			}
			catch (Exception ex)
			{
				alerta.IntentosEnvio++;
				alerta.ErrorEnvio = ex.Message;

				if (alerta.IntentosEnvio >= 3)
				{
					alerta.Estado = EstadoNotificacion.FALLIDA;
					logger.LogWarning("Alerta {AlertaId} marcada como FALLIDA luego de {Intentos} intentos.", alerta.AlertaId, alerta.IntentosEnvio);
				}
				else
				{
					logger.LogWarning(ex, "Fallo al enviar alerta {AlertaId}, intento {Intento}.", alerta.AlertaId, alerta.IntentosEnvio);
				}
			}

			await alertaRepository.UpdateAsync(alerta);
		}

		if (pendientes.Any())
		{
			await alertaRepository.SaveChangesAsync();
		}
	}

	public async Task<AlertaDto?> MarcarComoLeidaAsync(Guid alertaId)
	{
		var alerta = await alertaRepository.GetByIdAsync(alertaId);
		if (alerta == null)
		{
			return null;
		}

		if (alerta.FechaLectura == null)
		{
			alerta.FechaLectura = DateTimeOffset.UtcNow;
			if (alerta.Estado == EstadoNotificacion.ENVIADA || alerta.Estado == EstadoNotificacion.ENTREGADA)
			{
				alerta.Estado = EstadoNotificacion.LEIDA;
			}

			await alertaRepository.UpdateAsync(alerta);
			await alertaRepository.SaveChangesAsync();
		}

		return MapToDto(alerta);
	}

	public async Task<IEnumerable<AlertaDto>> ListarPorUsuarioAsync(Guid usuarioId, bool soloNoLeidas)
	{
		var alertas = await alertaRepository.GetByUsuarioAsync(usuarioId, soloNoLeidas);
		return alertas.Select(MapToDto);
	}

	private static AlertaDto MapToDto(Alerta alerta) => new()
	{
		AlertaId = alerta.AlertaId,
		UsuarioId = alerta.UsuarioId,
		ObligacionId = alerta.ObligacionId,
		TipoAlerta = alerta.TipoAlerta,
		Titulo = alerta.Titulo,
		Mensaje = alerta.Mensaje,
		Prioridad = alerta.Prioridad,
		MontoEstimado = alerta.MontoEstimado,
		Canal = alerta.Canal,
		Estado = alerta.Estado,
		AccionSugerida = alerta.AccionSugerida,
		FechaProgramada = alerta.FechaProgramada,
		FechaEnvio = alerta.FechaEnvio,
		FechaLectura = alerta.FechaLectura,
		IntentosEnvio = alerta.IntentosEnvio,
	};
}