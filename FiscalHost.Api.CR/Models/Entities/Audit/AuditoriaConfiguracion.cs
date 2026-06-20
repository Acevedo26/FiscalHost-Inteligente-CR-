namespace FiscalHost.Api.CR.Models.Entities.Audit;

// ========================================================================
// Entidad: Representa una tabla en la base de datos PostgreSQL, la cual se
// utiliza exclusivamente por Entity Framework para operaciones de lectura
// y escritura.
// ========================================================================


public class AuditoriaConfiguracion
{
    public int Id { get; set; }
    public int ConfiguracionTributariaId { get; set; }
    public ConfiguracionTributaria ConfiguracionTributaria { get; set; } = null!;

    public string Campo { get; set; } = string.Empty;
    public string ValorAnterior { get; set; } = string.Empty;
    public string ValorNuevo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; } = DateTime.UtcNow;
}


