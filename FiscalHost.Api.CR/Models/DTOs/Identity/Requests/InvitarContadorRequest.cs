using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Requests;

public class InvitarContadorRequest
{
    [Required]
    public Guid AnfitrionId { get; set; }

    [Required]
    [EmailAddress]
    public string CorreoContador { get; set; } = string.Empty;

    public bool PuedeVerIngresos { get; set; } = true;

    public bool PuedeVerGastos { get; set; } = true;

    public bool PuedeGenerarReportes { get; set; } = true;

    public DateTimeOffset? FechaExpiracion { get; set; }
}
