using FiscalHost.Api.CR.Models.DTOs.Operations.Requests;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Mvc;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/clasificacion-ingresos")]
public class ClasificacionIngresosController(
    IClasificacionIngresoService service) : ControllerBase
{
    [HttpPost("clasificar")]
    public async Task<IActionResult> Clasificar(
        [FromBody] ClasificarIngresoRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error, data) = await service.ClasificarAsync(request);

        return success
            ? Ok(data)
            : UnprocessableEntity(new { mensaje = error });
    }

    [HttpPut("{id:int}/reclasificar")]
    public async Task<IActionResult> Reclasificar(
        int id,
        [FromBody] ReclasificacionIngresoRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error, data) = await service.ReclasificarAsync(id, request);

        return success
            ? Ok(data)
            : UnprocessableEntity(new { mensaje = error });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);

        return result is null
            ? NotFound(new { mensaje = $"No se encontro clasificacion para el ingreso '{id}'." })
            : Ok(result);
    }
}
