namespace FiscalHost.Api.CR.Models.Entities.Audit;

// ========================================================================
// Entidad: Representa una tabla en la base de datos PostgreSQL, la cual se
// utiliza exclusivamente por Entity Framework para operaciones de lectura
// y escritura.
// ========================================================================


public class AuditoriaLlave
{
    public int Id { get; set; }
    public int LlaveCriptograficaId { get; set; }
    public LlaveCriptografica LlaveCriptografica { get; set; } = null!;
    public string Accion { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; } = DateTime.UtcNow;
}


