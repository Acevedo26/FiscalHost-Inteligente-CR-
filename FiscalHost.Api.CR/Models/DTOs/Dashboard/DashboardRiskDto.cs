using System.Collections.Generic;

namespace FiscalHost.Api.CR.Models.DTOs.Dashboard;

public class DashboardRiskDto
{
    public string NivelRiesgo { get; set; } = string.Empty;
    public List<string> Factores { get; set; } = new();
}
