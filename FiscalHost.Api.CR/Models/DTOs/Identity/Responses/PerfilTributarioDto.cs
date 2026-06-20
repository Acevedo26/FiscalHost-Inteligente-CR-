using System;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Responses;

// ========================================================================
// DTO de Response (Salida): Contiene la información formateada que la API
// le devuelve al frontend para mostrar en pantalla, ocultando, de esta
// manera, cualquier dato sensible.
// ========================================================================


public class PerfilTributarioDto
{
    public Guid PerfilId { get; set; }
    public Guid UsuarioId { get; set; }
    public string CodigoActividadEconomica { get; set; } = string.Empty;
    public string DescripcionActividad { get; set; } = string.Empty;
    public string? TribuCr { get; set; }
    public string? DireccionFiscal { get; set; }
    public string? Nise { get; set; }
    public bool EsDomicilioValidado { get; set; }
    public RegimenTributario RegimenTributario { get; set; }
    public DateTime? FechaInicioActividad { get; set; }
    public DateTime? FechaInscripcionHacienda { get; set; }
    public string DatosComplementarios { get; set; } = "{}";
}


