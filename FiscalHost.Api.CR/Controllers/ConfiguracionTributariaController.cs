using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Mvc;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/configuracion-tributaria")]
public class ConfiguracionTributariaController(IConfiguracionTributariaService service) : ControllerBase
{
    [HttpGet("actividades")]
    public async Task<IActionResult> GetActividades() =>
        Ok(await service.GetActividadesAsync());

    [HttpGet("{anfitrionId}")]
    public async Task<IActionResult> GetConfiguracion(string anfitrionId)
    {
        var result = await service.GetConfiguracionAsync(anfitrionId);
        return result is null ? NotFound($"No se encontró configuración para el anfitrión '{anfitrionId}'.") : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> GuardarConfiguracion([FromBody] ConfiguracionTributariaRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (success, error, data) = await service.GuardarConfiguracionAsync(request);

        if (!success) return UnprocessableEntity(new { mensaje = error });

        return Ok(data);
    }
}
