using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Requests;

public class ConfiguracionTributariaRequest
{
    [Required] public string AnfitrionId { get; set; } = string.Empty;
    [Required] public string CodigoActividad { get; set; } = string.Empty;
    [Required] public string DireccionFiscal { get; set; } = string.Empty;
    [Required] public string Nise { get; set; } = string.Empty;
}
