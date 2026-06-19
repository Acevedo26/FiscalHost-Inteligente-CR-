using System;
using System.ComponentModel.DataAnnotations;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

public class CreateObligacionTributariaRequest
{
    public Guid? CalculoId { get; set; }

    [Required]
    public Guid PeriodoId { get; set; }

    [Required]
    public TipoFormulario TipoFormulario { get; set; }

    [Required]
    public string Descripcion { get; set; } = string.Empty;

    [Required]
    public decimal MontoCapital { get; set; }

    [Required]
    public DateTime FechaVencimiento { get; set; }
}
