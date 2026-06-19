using System;
using System.ComponentModel.DataAnnotations;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Requests;

public class CreatePerfilTributarioRequest
{
    [Required]
    [MaxLength(20)]
    public string CodigoActividadEconomica { get; set; } = string.Empty;

    [Required]
    public string DescripcionActividad { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? TribuCr { get; set; }

    public string? DireccionFiscal { get; set; }

    [MaxLength(50)]
    public string? Nise { get; set; }

    [Required]
    public RegimenTributario RegimenTributario { get; set; }

    public DateTime? FechaInicioActividad { get; set; }
    
    public DateTime? FechaInscripcionHacienda { get; set; }
    
    public string DatosComplementarios { get; set; } = "{}";
}
