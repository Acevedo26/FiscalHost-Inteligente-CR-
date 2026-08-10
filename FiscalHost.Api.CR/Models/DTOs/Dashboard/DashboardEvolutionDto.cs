namespace FiscalHost.Api.CR.Models.DTOs.Dashboard;

public class DashboardEvolutionDto
{
    public int Anio { get; set; }
    public int Mes { get; set; }
    public decimal IngresosBrutos { get; set; }
    public decimal ImpuestosEstimados { get; set; }
    public decimal IngresosNetos { get; set; }
}
