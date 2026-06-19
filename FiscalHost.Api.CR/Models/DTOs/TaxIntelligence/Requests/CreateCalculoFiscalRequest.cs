using System;
using System.ComponentModel.DataAnnotations;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

public class CreateCalculoFiscalRequest
{
    [Required]
    public Guid PeriodoId { get; set; }

    [Required]
    public TipoFormulario TipoFormulario { get; set; }

    public RegimenTributario? RegimenAplicado { get; set; }
}
