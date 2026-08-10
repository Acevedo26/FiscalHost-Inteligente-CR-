using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Requests;

public class LoginRequest
{
    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    public required string Correo { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida")]
    public required string Contrasena { get; set; }
}
