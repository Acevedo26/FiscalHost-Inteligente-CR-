namespace FiscalHost.Api.CR.Models.Entities;

public class ActividadEconomica
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Activa { get; set; } = true;

    public ICollection<ConfiguracionTributaria> Configuraciones { get; set; } = [];
}
