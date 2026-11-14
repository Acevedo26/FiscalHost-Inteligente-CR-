using FiscalHost.Api.CR.Extensions;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/reconstruccion")]
public class ReconstruccionHistoricaController(IReconstruccionHistoricaService service) : ControllerBase
{
    [HttpPost("historica")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ReconstruirHistorica([FromForm] ReconstruccionHistoricaRequest request)
    {
        request.UsuarioId = User.GetUsuarioId();
        try
        {
            var resultado = await service.ReconstruirAsync(request);
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

    [HttpGet("periodos-faltantes")]
    public async Task<IActionResult> DetectarPeriodosFaltantes(
        [FromQuery] short anioInicio,
        [FromQuery] short anioFin,
        [FromQuery] string tipoFormulario)
    {
        var usuarioId = User.GetUsuarioId();

        if (!Enum.TryParse<TipoFormulario>(tipoFormulario, true, out var tipo))
        {
            return BadRequest(new
            {
                Success = false,
                Error = $"TipoFormulario '{tipoFormulario}' inválido. Valores permitidos: D104, D125, D150, D116, D176."
            });
        }

        var request = new DetectarPeriodosFaltantesRequest
        {
            UsuarioId = usuarioId,
            AnioInicio = anioInicio,
            AnioFin = anioFin,
            TipoFormulario = tipo
        };

        try
        {
            var resultado = await service.DetectarPeriodosFaltantesAsync(request);
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

    [HttpGet("{reconstruccionId}/errores")]
    public async Task<IActionResult> DescargarErrores(Guid reconstruccionId)
    {
        try
        {
            var contenido = await service.ObtenerReporteErroresAsync(reconstruccionId);

            return File(
                Encoding.UTF8.GetBytes(contenido),
                "text/csv",
                $"errores_reconstruccion_{reconstruccionId}.csv");
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
}