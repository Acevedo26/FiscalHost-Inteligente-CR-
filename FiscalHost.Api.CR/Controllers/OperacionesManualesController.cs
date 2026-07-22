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

    // ========================================================================
    // Endpoints HU-007
    // ========================================================================

    /// <summary>
    /// Sube un comprobante, extrae metadatos mediante OCR y lo guarda de forma segura.
    /// </summary>
    [HttpPost("gasto/comprobante")]
    public async Task<IActionResult> SubirComprobanteGasto(
        [FromForm] UploadComprobanteRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (success, error) = await service.SubirComprobanteGastoAsync(request);

        if (!success) return UnprocessableEntity(new { mensaje = error });

        return Ok(new { mensaje = "Comprobante procesado y guardado exitosamente." });
    }

    /// <summary>
    /// Actualiza un gasto existente. Exige justificación por Ley 8968.
    /// </summary>
    [HttpPut("gasto/{id:guid}")]
    public async Task<IActionResult> ActualizarGasto(
        Guid id, [FromBody] UpdateGastoRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (success, error) = await service.ActualizarGastoAsync(id, request);

        if (!success) return UnprocessableEntity(new { mensaje = error });

        return Ok(new { mensaje = "Gasto actualizado y auditado correctamente." });
    }

    /// <summary>
    /// Elimina un gasto existente. Exige justificación por Ley 8968.
    /// </summary>
    [HttpDelete("gasto/{id:guid}")]
    public async Task<IActionResult> EliminarGasto(
        Guid id, [FromBody] DeleteGastoRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (success, error) = await service.EliminarGastoAsync(id, request);

        if (!success) return UnprocessableEntity(new { mensaje = error });

        return Ok(new { mensaje = "Gasto eliminado y auditado correctamente." });
    }
}
