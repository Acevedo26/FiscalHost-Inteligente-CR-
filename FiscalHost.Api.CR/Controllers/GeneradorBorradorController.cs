using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FiscalHost.Api.CR.Services;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/borradores")]
public class GeneradorBorradorController(IGeneradorBorradorService service) : ControllerBase
{
    [HttpPost("d104")]
    public async Task<ActionResult<BorradorD104Dto>> GetBorradorD104([FromBody] GenerarBorradorD104Request request)
    {
        try
        {
            var borrador = await service.GenerarD104Async(request.UsuarioId, request.Anio, request.Mes);
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

    [HttpPost("d125")]
    public async Task<ActionResult<BorradorD125Dto>> GetBorradorD125([FromBody] GenerarBorradorD125Request request)
    {
        try
        {
            var borrador = await service.GenerarD125Async(request.UsuarioId, request.Anio, request.RegimenUtilidades);
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

    [HttpPost("d176")]
    public ActionResult<BorradorD176Dto> GetBorradorD176([FromBody] GenerarBorradorD176Request request)
    {
        try
        {
            var borrador = service.GenerarD176(request.ImpuestoPrincipal, request.FechaVencimientoOriginal, DateOnly.FromDateTime(DateTime.UtcNow));
            return Ok(borrador);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Ocurrió un error al generar el borrador.", detalle = ex.Message });
        }
    }
}
