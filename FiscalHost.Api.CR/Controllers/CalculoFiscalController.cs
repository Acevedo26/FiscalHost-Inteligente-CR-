using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence;
using FiscalHost.Api.CR.Services;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalculoFiscalController : ControllerBase
{
    private readonly ICalculoIvaService _calculoIvaService;

    public CalculoFiscalController(ICalculoIvaService calculoIvaService)
    {
        _calculoIvaService = calculoIvaService;
    }

    [HttpPost("iva/generar")]
    public async Task<IActionResult> GenerarCalculoIva([FromQuery] Guid usuarioId, [FromQuery] short anio, [FromQuery] short mes)
    {
        try
        {
            var resultado = await _calculoIvaService.CalcularIvaDevengadoAsync(usuarioId, anio, mes);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error interno del servidor.", Detalle = ex.Message });
        }
    }
}
