using System;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

public class BorradorD104Dto
{
    public decimal TotalIngresosGravados { get; set; }
    public decimal IvaCobrado { get; set; }
    public decimal IvaCreditoFiscal { get; set; }
    public decimal IvaNeto { get; set; }
    public bool EsSaldoAFavor { get; set; }
    public string? MensajeValidacion { get; set; }
}
