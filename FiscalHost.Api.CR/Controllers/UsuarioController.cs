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
}
