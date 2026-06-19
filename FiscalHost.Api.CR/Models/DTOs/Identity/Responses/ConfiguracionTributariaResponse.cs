namespace FiscalHost.Api.CR.Models.DTOs.Identity.Responses;

public class ConfiguracionTributariaResponse
{
    public int Id { get; set; }
    public string AnfitrionId { get; set; } = string.Empty;
    public string CodigoActividad { get; set; } = string.Empty;
    public string DescripcionActividad { get; set; } = string.Empty;
    public string TribuCr { get; set; } = string.Empty;
    public string DireccionFiscal { get; set; } = string.Empty;
    public string Nise { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaActualizacion { get; set; }
    public string? Advertencia { get; set; }
}
