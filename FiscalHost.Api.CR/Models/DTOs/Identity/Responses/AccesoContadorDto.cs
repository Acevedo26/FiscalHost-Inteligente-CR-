using System;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Responses;

// ========================================================================
// DTO de Response (Salida): Contiene la información formateada que la API
// le devuelve al frontend para mostrar en pantalla, ocultando, de esta
// manera, cualquier dato sensible.
// ========================================================================


public class AccesoContadorDto
{
    public Guid AccesoId { get; set; }
    public Guid AnfitrionId { get; set; }
    public Guid? ContadorId { get; set; }
    public string CorreoContador { get; set; } = string.Empty;
    public string Permisos { get; set; } = "{}";
    public DateTimeOffset FechaInvitacion { get; set; }
    public DateTimeOffset? FechaAceptacion { get; set; }
    public DateTimeOffset? FechaExpiracion { get; set; }
    public DateTimeOffset? FechaRevocacion { get; set; }
    public string Estado { get; set; } = string.Empty;
}


