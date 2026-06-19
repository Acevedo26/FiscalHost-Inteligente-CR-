using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Requests;

public class GastoOperativoRequest
{
    [Required]
    public string AnfitrionId { get; set; } = string.Empty;

    [Required]
    public string Proveedor { get; set; } = string.Empty;

    [Required]
    public string NumeroFactura { get; set; } = string.Empty;

    [Required]
    public decimal Monto { get; set; }

    public string? ComprobanteUrl { get; set; }

    [Required]
    public DateTime FechaGasto { get; set; }
}
