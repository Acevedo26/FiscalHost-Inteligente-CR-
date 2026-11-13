using FiscalHost.Api.CR.Models.DTOs.Identity.Requests;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace FiscalHost.Api.CR.Controllers;

[ApiController] 
[Route("api/auth")]
[AllowAnonymous]
public class AuthController(IAuthService service) : ControllerBase
{
    [HttpPost("register")]
    [HttpPost("registro")]
    public async Task<IActionResult> Register(
        [FromBody] RegistroUsuarioRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState); 

        var (success, error, data) =
            await service.RegistrarUsuarioAsync(request);

        if (!success)
            return UnprocessableEntity(new
            {
                mensaje = error
            });

        return Ok(data); 
    }
     
    [HttpPost("login")] 
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error, data) =
            await service.LoginAsync(request);

        if (!success)
            return Unauthorized(new
            {
                mensaje = error
            });

        return Ok(data);
    }
}
