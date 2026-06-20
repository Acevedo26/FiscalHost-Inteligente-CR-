using System;
using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

// ========================================================================
// DTO de Request (Entrada): Contiene, únicamente, los datos que el usuario
// o frontend envía a la API al realizar una petición, sin incluir
// identificadores generados ni campos de auditoría internos.
// ========================================================================


public class CreateExportacionRequest
{
    public Guid? CalculoId { get; set; }

    [Required]
    public string Formato { get; set; } = string.Empty;

    [Required]
    public string TipoContenido { get; set; } = string.Empty;

    public bool EstaProtegido { get; set; }
}


