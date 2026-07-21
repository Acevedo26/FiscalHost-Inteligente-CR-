using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FiscalHost.Api.CR.Services;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/borradores")]
public class GeneradorBorradorController(IGeneradorBorradorService service) : ControllerBase
{
    [HttpGet("d104")]
    public async Task<ActionResult<BorradorD104Dto>> GetBorradorD104([FromQuery] Guid usuarioId, [FromQuery] short anio, [FromQuery] short mes)
    {
        try
        {
            var borrador = await service.GenerarD104Async(usuarioId, anio, mes);
            return Ok(borrador);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Ocurrió un error al generar el borrador.", detalle = ex.Message });
        }
    }

    [HttpGet("d125")]
    public async Task<ActionResult<BorradorD125Dto>> GetBorradorD125([FromQuery] Guid usuarioId, [FromQuery] short anio, [FromQuery] bool regimenUtilidades = false)
    {
        try
        {
            var borrador = await service.GenerarD125Async(usuarioId, anio, regimenUtilidades);
            return Ok(borrador);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Ocurrió un error al generar el borrador.", detalle = ex.Message });
        }
    }

    [HttpGet("d176")]
    public ActionResult<BorradorD176Dto> GetBorradorD176([FromQuery] decimal impuestoPrincipal, [FromQuery] DateOnly fechaVencimientoOriginal)
    {
        try
        {
            var borrador = service.GenerarD176(impuestoPrincipal, fechaVencimientoOriginal, DateOnly.FromDateTime(DateTime.UtcNow));
            return Ok(borrador);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Ocurrió un error al generar el borrador.", detalle = ex.Message });
        }
    }
}
