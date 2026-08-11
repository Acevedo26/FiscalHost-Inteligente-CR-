using FiscalHost.Api.CR.Models.DTOs.Identity.Requests;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Mvc;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/accesos-contadores")]
public class AccesosContadoresController(
    IAccesoContadorService service)
    : ControllerBase
{
    [HttpPost("invitar")]
    public async Task<IActionResult> Invitar([FromBody] InvitarContadorRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error, data) = await service.InvitarAsync(request);

        return success
            ? Ok(data)
            : UnprocessableEntity(new { mensaje = error });
    }

    [HttpGet("anfitrion/{anfitrionId:guid}")]
    public async Task<IActionResult> GetByAnfitrion(Guid anfitrionId)
    {
        var accesos = await service.GetByAnfitrionAsync(anfitrionId);
        return Ok(accesos);
    }

    [HttpGet("validar")]
    public async Task<IActionResult> ValidarPermiso(
        [FromQuery] Guid anfitrionId,
        [FromQuery] string correoContador,
        [FromQuery] string permiso)
    {
        var (autorizado, mensaje) = await service.ValidarPermisoAsync(
            anfitrionId,
            correoContador,
            permiso);

        return autorizado
            ? Ok(new { autorizado, mensaje })
            : StatusCode(StatusCodes.Status403Forbidden, new { autorizado, mensaje });
    }

    [HttpPut("{accesoId:guid}/revocar")]
    public async Task<IActionResult> Revocar(
        Guid accesoId,
        [FromBody] RevocarAccesoContadorRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await service.RevocarAsync(accesoId, request);

        return success
            ? Ok(new { mensaje = "Acceso revocado correctamente." })
            : UnprocessableEntity(new { mensaje = error });
    }

    [HttpPost("procesar-expiraciones")]
    public async Task<IActionResult> ProcesarExpiraciones()
    {
        var total = await service.ProcesarExpiracionesAsync();
        return Ok(new { accesosExpirados = total });
    }
}
