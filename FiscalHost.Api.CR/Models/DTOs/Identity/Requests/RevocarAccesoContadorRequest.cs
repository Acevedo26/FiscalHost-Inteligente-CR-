using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Requests;

public class RevocarAccesoContadorRequest
{
    [Required]
    public Guid AnfitrionId { get; set; }

    [Required]
    public string Justificacion { get; set; } = string.Empty;
}
