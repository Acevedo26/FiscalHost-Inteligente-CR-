using System;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Responses;

// ========================================================================
// DTO de Response (Salida): Contiene la información formateada que la API
// le devuelve al frontend para mostrar en pantalla, ocultando, de esta
// manera, cualquier dato sensible.
// ========================================================================


public class UsuarioDto
{
    public Guid UsuarioId { get; set; }
    public TipoIdentificacion TipoIdentificacion { get; set; }
    public string NumeroIdentificacion { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string? RazonSocial { get; set; }
    public string CorreoElectronico { get; set; } = string.Empty;
    public EstadoUsuario Estado { get; set; }
    public RolUsuario RolPrincipal { get; set; }
    public bool EsUsuarioNuevo { get; set; }
    public bool CorreoVerificado { get; set; }
    public string PreferenciasNotificacion { get; set; } = "{}";
    public DateTimeOffset? FechaActivacion { get; set; }
    public DateTimeOffset? UltimoAcceso { get; set; }
}


