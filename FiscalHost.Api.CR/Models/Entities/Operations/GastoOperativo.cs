namespace FiscalHost.Api.CR.Models.Entities.Operations;

public class GastoOperativo
{
    public int Id { get; set; }

    public string AnfitrionId { get; set; } = string.Empty;

    public string Proveedor { get; set; } = string.Empty;

    public string NumeroFactura { get; set; } = string.Empty;

    public decimal Monto { get; set; }

    public string? ComprobanteUrl { get; set; }

    public DateTime FechaGasto { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
