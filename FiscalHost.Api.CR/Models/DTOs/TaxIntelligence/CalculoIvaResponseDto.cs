using System;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence;

public class CalculoIvaResponseDto
{
    public Guid CalculoId { get; set; }
    public Guid PeriodoId { get; set; }
    public decimal TotalIngresosBrutos { get; set; }
    public decimal TotalIngresosGravados { get; set; }
    public decimal TotalIngresosExentos { get; set; }
    public decimal DebitoFiscal { get; set; }
    public decimal CreditoFiscal { get; set; }
    public decimal SaldoFavorAnterior { get; set; }
    public decimal IvaNeto { get; set; }
    public decimal SaldoFavorResultante { get; set; }
    public decimal MontoTotalAPagar { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string DetalleCalculo { get; set; } = "{}";
    public DateTimeOffset CreatedAt { get; set; }
}
