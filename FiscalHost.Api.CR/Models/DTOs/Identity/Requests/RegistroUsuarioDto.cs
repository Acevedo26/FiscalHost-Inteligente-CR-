using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Requests;

public class RegistroUsuarioRequest
{
    [Required]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string CorreoElectronico { get; set; } = string.Empty;

    [Required]
    public string Contrasena { get; set; } = string.Empty;

    [Required]
    public string NumeroIdentificacion { get; set; } = string.Empty;

    [Required]
    public string TipoIdentificacion { get; set; } = string.Empty;

    public string? RazonSocial { get; set; }
}

public class RegistroUsuarioResponse
{
    public Guid UsuarioId { get; set; }

    public string CorreoElectronico { get; set; } = string.Empty;

    public string Mensaje { get; set; } = string.Empty;
}
