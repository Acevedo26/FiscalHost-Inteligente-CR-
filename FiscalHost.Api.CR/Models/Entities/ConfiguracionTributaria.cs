using FiscalHost.Api.CR.Models.Emums;

namespace FiscalHost.Api.CR.Models.Entities;

public class ConfiguracionTributaria
{
    public int Id { get; set; }
    public string AnfitrionId { get; set; } = string.Empty;

    // Actividad económica
    public int ActividadEconomicaId { get; set; }
    public ActividadEconomica ActividadEconomica { get; set; } = null!;

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
