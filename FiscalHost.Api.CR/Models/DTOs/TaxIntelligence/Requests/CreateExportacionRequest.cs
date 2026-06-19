using System;
using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

public class CreateExportacionRequest
{
    public Guid? CalculoId { get; set; }

    [Required]
    public string Formato { get; set; } = string.Empty;

    [Required]
    public string TipoContenido { get; set; } = string.Empty;

    public bool EstaProtegido { get; set; }
}
