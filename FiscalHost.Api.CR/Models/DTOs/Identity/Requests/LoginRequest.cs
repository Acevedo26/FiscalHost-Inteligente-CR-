using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Requests;

public class LoginRequest
{
    private string _contrasena = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    public required string Correo { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida")]
    public string Contrasena
    {
        get => !string.IsNullOrWhiteSpace(_contrasena) ? _contrasena : (Clave ?? string.Empty);
        set => _contrasena = value;
    }

    public string? Clave
    {
        get => _contrasena;
        set => _contrasena = value ?? string.Empty;
    }
}
