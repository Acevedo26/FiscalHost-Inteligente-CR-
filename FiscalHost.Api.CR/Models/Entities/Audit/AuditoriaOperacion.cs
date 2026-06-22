namespace FiscalHost.Api.CR.Models.Entities.Audit;

// ========================================================================
// Entidad: Representa una tabla en la base de datos PostgreSQL, la cual se
// utiliza exclusivamente por Entity Framework para operaciones de lectura
// y escritura.
// ========================================================================


public class AuditoriaOperacion
{
    public int Id { get; set; }

    public string Entidad { get; set; } = string.Empty;

    public int EntidadId { get; set; }

    public string Usuario { get; set; } = string.Empty;

    public string Accion { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    // Se agregan campos para auditoría estricta según Ley 8968 (CR)
    // Permiten conocer qué valor cambió específicamente.
    public string? ValorAnterior { get; set; }
    
    public string? ValorNuevo { get; set; }
    
    // Campo obligatorio en caso de actualizaciones o eliminaciones (Law 8968)
    public string? Justificacion { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}


