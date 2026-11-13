using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Requests;

// ========================================================================
// DTO de Request & Response para Registro de Usuarios
// ========================================================================

public class RegistroUsuarioRequest
{
    private string _nombreCompleto = string.Empty;
    private string _correoElectronico = string.Empty;
    private string _contrasena = string.Empty;
    private string _numeroIdentificacion = string.Empty;
    private string _tipoIdentificacion = "FISICA";

    [Required(ErrorMessage = "El nombre es requerido.")]
    public string NombreCompleto
    {
        get => !string.IsNullOrWhiteSpace(_nombreCompleto) ? _nombreCompleto : (Nombre ?? string.Empty);
        set => _nombreCompleto = value;
    }

    public string? Nombre
    {
        get => _nombreCompleto;
        set => _nombreCompleto = value ?? string.Empty;
    }

    [Required(ErrorMessage = "El correo electrónico es requerido.")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
    public string CorreoElectronico
    {
        get => !string.IsNullOrWhiteSpace(_correoElectronico) ? _correoElectronico : (Correo ?? string.Empty);
        set => _correoElectronico = value;
    }

    public string? Correo
    {
        get => _correoElectronico;
        set => _correoElectronico = value ?? string.Empty;
    }

    [Required(ErrorMessage = "La contraseña es requerida.")]
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

    [Required(ErrorMessage = "El número de identificación es requerido.")]
    public string NumeroIdentificacion
    {
        get => !string.IsNullOrWhiteSpace(_numeroIdentificacion) ? _numeroIdentificacion : (!string.IsNullOrWhiteSpace(Identificacion) ? Identificacion : "100000000");
        set => _numeroIdentificacion = value;
    }

    public string? Identificacion
    {
        get => _numeroIdentificacion;
        set => _numeroIdentificacion = value ?? string.Empty;
    }

    [Required(ErrorMessage = "El tipo de identificación es requerido.")]
    public string TipoIdentificacion
    {
        get => !string.IsNullOrWhiteSpace(_tipoIdentificacion) ? _tipoIdentificacion : "FISICA";
        set => _tipoIdentificacion = value;
    }

    public string? RazonSocial { get; set; }
}

public class RegistroUsuarioResponse
{
    public Guid Id => UsuarioId;

    [JsonIgnore]
    public Guid UsuarioId { get; set; }

    public string CorreoElectronico { get; set; } = string.Empty;

    public string Correo => CorreoElectronico;

    public string Mensaje { get; set; } = string.Empty;
}
