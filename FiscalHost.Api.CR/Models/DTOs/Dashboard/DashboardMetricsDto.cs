namespace FiscalHost.Api.CR.Models.DTOs.Dashboard;

public class DashboardMetricsDto
{
    public decimal IngresosBrutos { get; set; }
    public decimal ImpuestosEstimados { get; set; }
    public decimal IvaDebito { get; set; }
    public decimal IvaCreditoFiscal { get; set; }
    public decimal IvaRetenido { get; set; }
    public decimal RentaEstimada { get; set; }
    public decimal GastosTotales { get; set; }
    public decimal IngresosNetos { get; set; }
}
