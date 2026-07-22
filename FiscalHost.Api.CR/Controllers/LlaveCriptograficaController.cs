using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Mvc;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/llaves-criptograficas")]
public class LlaveCriptograficaController(ILlaveCriptograficaService service) : ControllerBase
{
    [HttpPost("cargar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Cargar([FromForm] CargarLlaveRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var (success, error, data) = await service.CargarLlaveAsync(request);
        return success ? Ok(data) : UnprocessableEntity(new { mensaje = error });
    }

    [HttpPut("actualizar-contrasena")]
    public async Task<IActionResult> ActualizarContrasena([FromBody] ActualizarContrasenaRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var (success, error) = await service.ActualizarContrasenaAsync(request);
        return success ? Ok(new { mensaje = "Contraseña actualizada correctamente." }) : UnprocessableEntity(new { mensaje = error });
    }

    [HttpGet("{anfitrionId}")]
    public async Task<IActionResult> GetLlave(string anfitrionId)
    {
        var result = await service.GetLlaveAsync(anfitrionId);
        return result is null ? NotFound(new { mensaje = $"No se encontró llave activa para '{anfitrionId}'." }) : Ok(result);
    }
}
