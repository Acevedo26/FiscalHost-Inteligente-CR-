namespace FiscalHost.Api.CR.Models.DTOs.Operations.Responses;

public class ClasificacionIngresoResponse
{
    public int Id { get; set; }
    public string AnfitrionId { get; set; } = string.Empty;
    public int DiasEstancia { get; set; }
    public decimal MontoBruto { get; set; }
    public string FuenteIngreso { get; set; } = string.Empty;
    public string ClasificacionIva { get; set; } = string.Empty;
    public decimal MontoIva { get; set; }
    public decimal BaseImponibleRenta { get; set; }
    public decimal ImpuestoRenta { get; set; }
    public decimal MontoRetencion { get; set; }
    public decimal NetoAnfitrion { get; set; }
    public bool ReclasificadoManualmente { get; set; }
    public string? JustificacionManual { get; set; }
    public DateTime FechaActualizacion { get; set; }
}
