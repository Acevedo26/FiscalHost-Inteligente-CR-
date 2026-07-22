using System.Text;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Mvc;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/reconstrucciones-bases")]
public class ReconstruccionesBasesController(
    IReconstruccionBaseImponibleService service)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Reconstruir(
        [FromBody] ReconstruccionBaseImponibleRequest request)
    {
        var resultado = await service.ReconstruirAsync(request);
        return resultado.Success ? Ok(resultado) : UnprocessableEntity(resultado);
    }

    [HttpPost("validar-historico")]
    [Consumes("multipart/form-data")]
    public IActionResult ValidarHistorico([FromForm] IFormFile archivo)
    {
        var resultado = service.ValidarArchivoHistorico(archivo);
        return resultado.Success ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("plantilla")]
    public IActionResult DescargarPlantilla()
    {
        var contenido = service.GenerarPlantillaHistoricosCsv();

        return File(
            Encoding.UTF8.GetBytes(contenido),
            "text/csv",
            "plantilla_historicos_bases_imponibles.csv");
    }
}
