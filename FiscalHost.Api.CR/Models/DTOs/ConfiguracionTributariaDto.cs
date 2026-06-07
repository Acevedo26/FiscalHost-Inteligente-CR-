using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs;

public class ConfiguracionTributariaRequest
{
    [Required] public string AnfitrionId { get; set; } = string.Empty;
    [Required] public string CodigoActividad { get; set; } = string.Empty;
    [Required] public string DireccionFiscal { get; set; } = string.Empty;
    [Required] public string Nise { get; set; } = string.Empty;
}

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

public class ActividadEconomicaResponse
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}
