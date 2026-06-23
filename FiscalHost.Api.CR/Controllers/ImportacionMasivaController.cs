using FiscalHost.Api.CR.DTOs.Operations;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/importaciones")]
public class ImportacionMasivaController(
	IImportacionMasivaService service)
	: ControllerBase
{
	[HttpPost("csv")]
	[Consumes("multipart/form-data")]
	public async Task<IActionResult> ImportarCsv(
		[FromForm] ImportacionRequest request)
	{
		try
		{
			var resultado = await service.ImportarAsync(
				request.Archivo,
				request.UsuarioId);

			return Ok(resultado);
		}
		catch (Exception ex)
		{
			return BadRequest(new
			{
				Success = false,
				Error = ex.Message
			});
		}
	}

	[HttpGet("{importacionId}/errores")]
	public async Task<IActionResult> DescargarErrores(Guid importacionId)
	{
		try
		{
			var contenido = await service.ObtenerReporteErroresCsvAsync(importacionId);

			return File(
				Encoding.UTF8.GetBytes(contenido),
				"text/csv",
				$"errores_importacion_{importacionId}.csv");
		}
		catch (Exception ex)
		{
			return BadRequest(new
			{
				Success = false,
				Error = ex.Message
			});
		}
	}

	[HttpGet("plantilla")]
	public IActionResult DescargarPlantilla()
	{
		var contenido = service.GenerarPlantillaCsv();

		return File(
			Encoding.UTF8.GetBytes(contenido),
			"text/csv",
			"plantilla_importacion_reservas.csv");
	}
}