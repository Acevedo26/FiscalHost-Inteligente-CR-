using System;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.DTOs.Dashboard;

namespace FiscalHost.Api.CR.Services;

public interface IDashboardService
{
    Task<DashboardResponseDto> GetDashboardDataAsync(Guid usuarioId, DateTime fechaInicio, DateTime fechaFin);
}
