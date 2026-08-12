using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/taxintelligence/[controller]")]
[Authorize]
public class SimuladorFiscalController : ControllerBase
{
    private readonly ISimulacionFiscalService _simulacionService;

    public SimuladorFiscalController(ISimulacionFiscalService simulacionService)
    {
        _simulacionService = simulacionService;
    }

    private Guid GetUsuarioId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(idClaim, out Guid id))
            return id;
        throw new UnauthorizedAccessException("Usuario no autorizado.");
    }

    [HttpPost]
    public async Task<ActionResult<SimulacionFiscalResponseDto>> CrearSimulacion([FromBody] CreateSimulacionFiscalRequest request)
    {
        try
        {
            var usuarioId = GetUsuarioId();
            var response = await _simulacionService.CrearSimulacionAsync(usuarioId, request);
            return CreatedAtAction(nameof(ObtenerSimulacion), new { id = response.SimulacionId }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SimulacionFiscalResponseDto>> ObtenerSimulacion(Guid id)
    {
        try
        {
            var usuarioId = GetUsuarioId();
            var response = await _simulacionService.ObtenerSimulacionAsync(id, usuarioId);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SimulacionFiscalResponseDto>>> ListarSimulaciones()
    {
        var usuarioId = GetUsuarioId();
        var response = await _simulacionService.ListarSimulacionesAsync(usuarioId);
        return Ok(response);
    }

    [HttpGet("comparar")]
    public async Task<ActionResult<ComparacionSimulacionesResponseDto>> CompararSimulaciones([FromQuery] List<Guid> ids)
    {
        try
        {
            var usuarioId = GetUsuarioId();
            var response = await _simulacionService.CompararSimulacionesAsync(usuarioId, ids);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/exportar")]
    public async Task<IActionResult> ExportarSimulacion(Guid id)
    {
        try
        {
            var usuarioId = GetUsuarioId();
            var csvBytes = await _simulacionService.ExportarSimulacionCsvAsync(id, usuarioId);
            return File(csvBytes, "text/csv", $"Simulacion_{id}.csv");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarSimulacion(Guid id)
    {
        try
        {
            var usuarioId = GetUsuarioId();
            await _simulacionService.EliminarSimulacionAsync(id, usuarioId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
