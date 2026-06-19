namespace FiscalHost.Api.CR.Models.Entities.Audit;

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
