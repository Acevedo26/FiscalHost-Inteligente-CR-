using System.Collections.Generic;

namespace FiscalHost.Api.CR.Models.DTOs.Dashboard;

public class DashboardResponseDto
{
    public DashboardMetricsDto Metricas { get; set; } = new();
    public DashboardRiskDto RiesgoFiscal { get; set; } = new();
    public List<DashboardEvolutionDto> EvolucionMensual { get; set; } = new();
    public bool TieneDatos { get; set; }
}
