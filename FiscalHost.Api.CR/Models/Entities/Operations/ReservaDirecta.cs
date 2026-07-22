namespace FiscalHost.Api.CR.Models.Entities.Operations;

// ========================================================================
// Entidad: Representa una tabla en la base de datos PostgreSQL, la cual se
// utiliza exclusivamente por Entity Framework para operaciones de lectura
// y escritura.
// ========================================================================


public class ReservaDirecta
{
    public int Id { get; set; }

    public string AnfitrionId { get; set; } = string.Empty;

    public DateTime FechaReserva { get; set; }

    public decimal Monto { get; set; }

    public string Huesped { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}


