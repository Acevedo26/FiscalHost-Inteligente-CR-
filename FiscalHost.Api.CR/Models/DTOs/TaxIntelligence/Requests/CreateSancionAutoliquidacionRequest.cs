using System;
using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

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
