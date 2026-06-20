using System;
using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

// ========================================================================
// DTO de Request (Entrada): Contiene, únicamente, los datos que el usuario
// o frontend envía a la API al realizar una petición, sin incluir
// identificadores generados ni campos de auditoría internos.
// ========================================================================


public class CreateSancionAutoliquidacionRequest
{
    public Guid? ObligacionId { get; set; }

    [Required]
    public string TipoSancion { get; set; } = string.Empty;

    [Required]
    public string Descripcion { get; set; } = string.Empty;

    [Required]
    public DateTime FechaOmision { get; set; }

    [Required]
    public decimal MontoBaseAdeudado { get; set; }
}


