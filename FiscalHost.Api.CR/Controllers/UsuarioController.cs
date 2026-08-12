using System;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.DTOs.Identity.Requests;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Mvc;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuarioController(IUsuarioService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var usuarios = await service.ObtenerTodosAsync();
        return Ok(usuarios);
    }

	[HttpGet("{id:guid}")]
	public async Task<IActionResult> GetById(Guid id)
	{
		var usuario = await service.ObtenerPorIdAsync(id);
		if (usuario == null)
		{
			return NotFound(new { error = "Usuario no encontrado." });
		}
		return Ok(usuario);
	}

	[HttpGet("{id:guid}/preferencias-notificacion")]
	public async Task<IActionResult> ObtenerPreferenciasNotificacion(Guid id)
	{
		var preferencias = await service.ObtenerPreferenciasNotificacionAsync(id);
		if (preferencias == null)
		{
			return NotFound(new { error = "Usuario no encontrado." });
		}
		return Ok(preferencias);
	}

	[HttpPost("{id:guid}/tutorial/completar")]
	public async Task<IActionResult> CompletarTutorial(Guid id)
	{
		var (success, error) = await service.MarcarTutorialCompletadoAsync(id);
		if (!success)
		{
			return NotFound(new { error });
		}
		return Ok(new { mensaje = "Tutorial marcado como completado." });
	}

	[HttpPut("{id:guid}/preferencias-notificacion")]
	public async Task<IActionResult> ActualizarPreferenciasNotificacion(
		Guid id, [FromBody] ActualizarPreferenciasNotificacionRequest request)
	{
		var (success, error, data) = await service.ActualizarPreferenciasNotificacionAsync(id, request);
		if (!success)
		{
			return NotFound(new { error });
		}

		return Ok(data);
	}
}
