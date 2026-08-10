using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FiscalHost.Api.CR.Models.DTOs.Communication.Requests;
using FiscalHost.Api.CR.Services;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/contenido-educativo")]
public class ContenidoEducativoController(IContenidoEducativoService contenidoService) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> ListarPorCategoria([FromQuery] string categoria)
	{
		if (string.IsNullOrWhiteSpace(categoria))
		{
			return BadRequest(new { error = "Debe indicar una categoría." });
		}

		var contenidos = await contenidoService.ListarPorCategoriaAsync(categoria, incluirNoPublicados: false);
		return Ok(contenidos);
	}

	[HttpGet("tutoriales")]
	public async Task<IActionResult> ListarTutorialesPrimerUso()
	{
		var tutoriales = await contenidoService.ListarTutorialesPrimerUsoAsync();
		return Ok(tutoriales);
	}

	[HttpGet("{slug}")]
	public async Task<IActionResult> ObtenerPorSlug(string slug)
	{
		var contenido = await contenidoService.ObtenerPorSlugAsync(slug, incluirNoPublicados: false);
		if (contenido == null)
		{
			// RF-019 - Escenario "Contenido inexistente": se informa al usuario
			// y se ofrecen las categorías disponibles como alternativa de consulta.
			var alternativas = await contenidoService.ObtenerCategoriasDisponiblesAsync();
			return NotFound(new
			{
				error = "Contenido educativo no encontrado.",
				categoriasDisponibles = alternativas
			});
		}

		return Ok(contenido);
	}

	[HttpGet("categorias")]
	public async Task<IActionResult> ListarCategoriasDisponibles()
	{
		var categorias = await contenidoService.ObtenerCategoriasDisponiblesAsync();
		return Ok(categorias);
	}

	[HttpPost]
	public async Task<IActionResult> Crear([FromBody] CreateContenidoEducativoRequest request, [FromQuery] Guid? autorId = null)
	{
		var (success, error, data) = await contenidoService.CrearAsync(request, autorId);
		if (!success)
		{
			return BadRequest(new { error });
		}

		return CreatedAtAction(nameof(ObtenerPorSlug), new { slug = data!.Slug }, data);
	}

	[HttpPut("{contenidoId:guid}")]
	public async Task<IActionResult> Actualizar(Guid contenidoId, [FromBody] ActualizarContenidoEducativoRequest request)
	{
		var (success, error, data) = await contenidoService.ActualizarAsync(contenidoId, request);
		if (!success)
		{
			return NotFound(new { error });
		}

		return Ok(data);
	}
}