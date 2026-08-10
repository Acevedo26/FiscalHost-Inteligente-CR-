using System;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

public class GenerarBorradorD176Request
{
    public decimal ImpuestoPrincipal { get; set; }
    public DateOnly FechaVencimientoOriginal { get; set; }
}
