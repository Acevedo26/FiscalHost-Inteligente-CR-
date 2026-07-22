using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FiscalHost.Api.CR.Services;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/obligaciones")]
public class ObligacionesTributariasController(
    IObligacionTributariaService obligacionService) : ControllerBase
{
    [HttpGet("{id:guid}/deuda")]
    public async Task<IActionResult> ConsultarDeuda(Guid id)
    {
        var deuda = await obligacionService.ConsultarDeudaAsync(id);
        if (deuda == null)
            return NotFound(new { error = "Obligación tributaria no encontrada." });

        if (deuda.FechaVencimiento == default)
        {
            return BadRequest(new { error = "La obligación no tiene una fecha de vencimiento válida, no se pueden calcular los intereses." });
        }

        return Ok(deuda);
    }

    [HttpPost("recalcular-mora")]
    public async Task<IActionResult> RecalcularMoraManual([FromQuery] DateTime? fechaFija = null)
    {
        // Solo para propósitos administrativos o pruebas manuales.
        // Permite simular el paso del tiempo enviando una fecha específica, o usar la fecha actual.
        var fechaCorte = fechaFija.HasValue ? DateOnly.FromDateTime(fechaFija.Value) : DateOnly.FromDateTime(DateTime.UtcNow);
        
        await obligacionService.ProcesarCargosMoratoriosMasivosAsync(fechaCorte);
        
        return Ok(new { mensaje = $"Cálculo de mora masivo ejecutado exitosamente con fecha de corte: {fechaCorte:yyyy-MM-dd}." });
    }
}
