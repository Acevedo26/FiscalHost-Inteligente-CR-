namespace FiscalHost.Api.CR.Models.Entities;

public class AuditoriaLlave
{
    public int Id { get; set; }
    public int LlaveCriptograficaId { get; set; }
    public LlaveCriptografica LlaveCriptografica { get; set; } = null!;
    public string Accion { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; } = DateTime.UtcNow;
}
