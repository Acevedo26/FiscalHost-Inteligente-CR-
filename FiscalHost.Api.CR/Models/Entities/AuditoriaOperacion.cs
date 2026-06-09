namespace FiscalHost.Api.CR.Models.Entities;

public class AuditoriaOperacion
{
    public int Id { get; set; }

    public string Entidad { get; set; } = string.Empty;

    public int EntidadId { get; set; }

    public string Usuario { get; set; } = string.Empty;

    public string Accion { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}