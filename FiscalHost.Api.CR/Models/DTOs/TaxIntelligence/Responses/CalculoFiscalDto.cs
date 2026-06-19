using System;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

public class CalculoFiscalDto
{
    public Guid CalculoId { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid PeriodoId { get; set; }
    public TipoFormulario TipoFormulario { get; set; }
    public RegimenTributario? RegimenAplicado { get; set; }
    public EstadoDeclaracion Estado { get; set; }
    public decimal TotalIngresosBrutos { get; set; }
    public decimal TotalIngresosGravados { get; set; }
    public decimal TotalIngresosExentos { get; set; }
    public decimal DebitoFiscal { get; set; }
    public decimal CreditoFiscal { get; set; }
    public decimal IvaNeto { get; set; }
    public decimal SaldoFavorAnterior { get; set; }
    public decimal SaldoFavorResultante { get; set; }
    public decimal? RentaBruta { get; set; }
    public decimal? DeduccionAplicada { get; set; }
    public decimal? RentaNeta { get; set; }
    public decimal? ImpuestoRenta { get; set; }
    public decimal? RetencionesAcreditadas { get; set; }
    public decimal MontoTotalAPagar { get; set; }
    public string DetalleCalculo { get; set; } = "{}";
    public bool BorradorGenerado { get; set; }
    public DateTimeOffset? FechaGeneracionBorrador { get; set; }
}
