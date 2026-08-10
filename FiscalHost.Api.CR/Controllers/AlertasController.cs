using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FiscalHost.Api.CR.Services;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/alertas")]
public class AlertasController(IAlertaService alertaService) : ControllerBase
{
	[HttpGet("usuario/{usuarioId:guid}")]
	public async Task<IActionResult> ListarPorUsuario(Guid usuarioId, [FromQuery] bool soloNoLeidas = false)
	{
		var alertas = await alertaService.ListarPorUsuarioAsync(usuarioId, soloNoLeidas);
		return Ok(alertas);
	}

	[HttpPost("{id:guid}/marcar-leida")]
	public async Task<IActionResult> MarcarComoLeida(Guid id)
	{
		var alerta = await alertaService.MarcarComoLeidaAsync(id);
		if (alerta == null)
		{
			return NotFound(new { error = "Alerta no encontrada." });
		}

		return Ok(alerta);
	}

	[HttpPost("generar")]
	public async Task<IActionResult> GenerarManual([FromQuery] DateTime? fechaFija = null)
	{
		var fechaActual = fechaFija.HasValue ? DateOnly.FromDateTime(fechaFija.Value) : DateOnly.FromDateTime(DateTime.UtcNow);

		await alertaService.GenerarAlertasVencimientoAsync(fechaActual);
		await alertaService.EnviarAlertasPendientesAsync(DateTimeOffset.UtcNow);

		return Ok(new { mensaje = $"Generación y envío de alertas ejecutado exitosamente con fecha de corte: {fechaActual:yyyy-MM-dd}." });
	}
}