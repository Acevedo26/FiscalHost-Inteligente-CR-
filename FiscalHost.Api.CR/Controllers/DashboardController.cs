using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FiscalHost.Api.CR.Services;
using FiscalHost.Api.CR.Models.DTOs.Dashboard;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardResponseDto>> GetDashboardData(
        [FromQuery] DateTime fechaInicio, 
        [FromQuery] DateTime fechaFin)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var usuarioId))
        {
            return Unauthorized("Usuario no autenticado o token inválido.");
        }

        if (fechaInicio > fechaFin)
        {
            return BadRequest("La fecha de inicio no puede ser posterior a la fecha de fin.");
        }

        var result = await _dashboardService.GetDashboardDataAsync(usuarioId, fechaInicio, fechaFin);
        return Ok(result);
    }
}
