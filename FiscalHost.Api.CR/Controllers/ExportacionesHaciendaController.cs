using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Mvc;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/exportaciones-hacienda")]
public class ExportacionesHaciendaController(
    IExportacionHaciendaService service)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Exportar(
        [FromBody] ExportacionHaciendaRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var resultado = await service.ExportarAsync(request);

        return resultado.Success
            ? Ok(resultado)
            : UnprocessableEntity(resultado);
    }
}
