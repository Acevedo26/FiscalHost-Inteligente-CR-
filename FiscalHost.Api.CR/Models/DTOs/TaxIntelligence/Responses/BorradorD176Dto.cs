using System;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

public class BorradorD176Dto
{
    public decimal ImpuestoPrincipal { get; set; }
    public decimal MultaBase { get; set; }
    public decimal MultaReducida { get; set; }
    public decimal InteresesMora { get; set; }
    public decimal TotalAPagar { get; set; }
    public string? MensajeValidacion { get; set; }
}
