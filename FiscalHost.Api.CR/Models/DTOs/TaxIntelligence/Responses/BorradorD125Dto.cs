using System;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

public class BorradorD125Dto
{
    public decimal IngresoBrutoAnual { get; set; }
    public decimal BaseImponible { get; set; }
    public decimal ImpuestoRenta { get; set; }
    public decimal RetencionesExtranjeras { get; set; }
    public decimal ImpuestoNeto { get; set; }
    public string? MensajeValidacion { get; set; }
}
