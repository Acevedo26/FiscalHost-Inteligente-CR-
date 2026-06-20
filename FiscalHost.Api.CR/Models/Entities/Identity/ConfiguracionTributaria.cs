namespace FiscalHost.Api.CR.Models.Entities.Identity;

// ========================================================================
// Entidad: Representa una tabla en la base de datos PostgreSQL, la cual se
// utiliza exclusivamente por Entity Framework para operaciones de lectura
// y escritura.
// ========================================================================


public class ConfiguracionTributaria
{
    public int Id { get; set; }
    public string AnfitrionId { get; set; } = string.Empty;

    // Actividad econÃ³mica â€” referencia al catÃ¡logo DGT (tabla catalogo_actividad_economica)
    public string CodigoActividad { get; set; } = string.Empty;
    public CatalogoActividadEconomica? ActividadEconomica { get; set; }

    // TRIBU-CR
    public string TribuCr { get; set; } = string.Empty;

    // Domicilio fiscal
    public string DireccionFiscal { get; set; } = string.Empty;
    public string Nise { get; set; } = string.Empty;

    public EstadoConfiguracion Estado { get; set; } = EstadoConfiguracion.Activa;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

    public ICollection<AuditoriaConfiguracion> Auditorias { get; set; } = [];
}


