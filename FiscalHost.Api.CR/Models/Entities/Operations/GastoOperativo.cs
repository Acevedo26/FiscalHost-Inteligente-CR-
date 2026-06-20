namespace FiscalHost.Api.CR.Models.Entities.Operations;

// ========================================================================
// Entidad: Representa una tabla en la base de datos PostgreSQL, la cual se
// utiliza exclusivamente por Entity Framework para operaciones de lectura
// y escritura.
// ========================================================================


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


