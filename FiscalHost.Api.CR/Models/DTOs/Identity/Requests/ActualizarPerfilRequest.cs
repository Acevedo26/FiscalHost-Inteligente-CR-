using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Requests;

public class ActualizarPerfilRequest
{
    [Required]
    [MaxLength(200)]
    public string NombreCompleto { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? RazonSocial { get; set; }

    [Required]
    public TipoIdentificacion TipoIdentificacion { get; set; }

    [Required]
    [MaxLength(50)]
    public string NumeroIdentificacion { get; set; } = string.Empty;
}