using FiscalHost.Api.CR.Extensions;
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

    [HttpGet("mi-configuracion")]
    public async Task<IActionResult> GetMiConfiguracion()
    {
        var anfitrionId = User.GetUsuarioId().ToString();
        var result = await service.GetConfiguracionAsync(anfitrionId);
        return result is null ? NotFound(new { error = "No se encontró configuración." }) : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> GuardarConfiguracion([FromBody] ConfiguracionTributariaRequest request)
    {
        request.AnfitrionId = User.GetUsuarioId().ToString();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (success, error, data) = await service.GuardarConfiguracionAsync(request);

        if (!success) return UnprocessableEntity(new { mensaje = error });

        return Ok(data);
    }
}
