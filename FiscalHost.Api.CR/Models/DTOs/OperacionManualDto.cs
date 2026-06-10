using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs;

public class ReservaDirectaRequest
{
    [Required]
    public string AnfitrionId { get; set; } = string.Empty;

    [Required]
    public DateTime FechaReserva { get; set; }

    [Required]
    public decimal Monto { get; set; }

    [Required]
    public string Huesped { get; set; } = string.Empty;
}

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