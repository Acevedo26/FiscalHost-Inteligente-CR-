namespace FiscalHost.Api.CR.Models.DTOs.Identity.Responses;

public class AccesoContadorResponse
{
    public Guid AccesoId { get; set; }
    public Guid AnfitrionId { get; set; }
    public Guid? ContadorId { get; set; }
    public string CorreoContador { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTimeOffset FechaInvitacion { get; set; }
    public DateTimeOffset? FechaExpiracion { get; set; }
    public DateTimeOffset? FechaRevocacion { get; set; }
    public bool PuedeVerIngresos { get; set; }
    public bool PuedeVerGastos { get; set; }
    public bool PuedeGenerarReportes { get; set; }
}
