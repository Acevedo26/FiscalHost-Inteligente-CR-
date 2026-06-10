using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs;

public class RegistroUsuarioRequest
{
    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;

    [Required]
    public string Contrasena { get; set; } = string.Empty;

    [Required]
    public string NumeroIdentificacion { get; set; } = string.Empty;

    [Required]
    public string TipoIdentificacion { get; set; } = string.Empty;
}

public class RegistroUsuarioResponse
{
    public int Id { get; set; }

    public string Correo { get; set; } = string.Empty;

    public string Mensaje { get; set; } = string.Empty;
}