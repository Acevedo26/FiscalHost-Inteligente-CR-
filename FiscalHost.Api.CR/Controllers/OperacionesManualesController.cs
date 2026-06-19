using FiscalHost.Api.CR.Models.DTOs;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Mvc;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/operaciones-manuales")]
public class OperacionesManualesController(
    IOperacionManualService service) : ControllerBase
{
    [HttpPost("reserva")]
    public async Task<IActionResult> RegistrarReserva(
        [FromBody] ReservaDirectaRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) =
            await service.RegistrarReservaAsync(request);

        if (!success)
            return UnprocessableEntity(new { mensaje = error });

        return Ok(new
        {
            mensaje = "Reserva registrada correctamente."
        });
    }

    [HttpPost("gasto")]
    public async Task<IActionResult> RegistrarGasto(
        [FromBody] GastoOperativoRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) =
            await service.RegistrarGastoAsync(request);

        if (!success)
            return UnprocessableEntity(new { mensaje = error });

        return Ok(new
        {
            mensaje = "Gasto operativo registrado correctamente."
        });
    }
}
