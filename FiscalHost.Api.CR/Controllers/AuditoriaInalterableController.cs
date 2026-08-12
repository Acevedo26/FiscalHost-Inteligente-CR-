using FiscalHost.Api.CR.Models.DTOs.Audit.Requests;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Mvc;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/auditoria")]
public class AuditoriaInalterableController(
    IAuditoriaInalterableService service)
    : ControllerBase
{
    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarAuditoriaRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error, data) = await service.RegistrarAsync(request);

        return success
            ? Ok(data)
            : UnprocessableEntity(new { mensaje = error });
    }

    [HttpGet("historial")]
    public async Task<IActionResult> Historial(
        [FromQuery] Guid? usuarioId,
        [FromQuery] string? tablaAfectada,
        [FromQuery] Guid? registroId)
    {
        var historial = await service.ConsultarHistorialAsync(usuarioId, tablaAfectada, registroId);
        return Ok(historial);
    }

    [HttpGet("exportar")]
    public async Task<IActionResult> Exportar(
        [FromQuery] Guid? usuarioId,
        [FromQuery] string? tablaAfectada,
        [FromQuery] Guid? registroId)
    {
        var resultado = await service.ExportarHistorialAsync(usuarioId, tablaAfectada, registroId);

        return resultado.Success
            ? Ok(resultado)
            : UnprocessableEntity(resultado);
    }
}
