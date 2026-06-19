using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.Entities.TaxIntelligence;

[Table("calculo_fiscal", Schema = "fiscalhost_db")]
public class CalculoFiscal
{
    [Key]
    [Column("calculo_id")]
    public Guid CalculoId { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [Column("periodo_id")]
    public Guid PeriodoId { get; set; }
    public PeriodoFiscal Periodo { get; set; } = null!;

    [Column("tipo_formulario")]
    public TipoFormulario TipoFormulario { get; set; }

    [Column("regimen_aplicado")]
    public RegimenTributario? RegimenAplicado { get; set; }

    [Column("estado")]
    public EstadoDeclaracion Estado { get; set; }

    [Column("total_ingresos_brutos")]
    public decimal TotalIngresosBrutos { get; set; }

    [Column("total_ingresos_gravados")]
    public decimal TotalIngresosGravados { get; set; }

    [Column("total_ingresos_exentos")]
    public decimal TotalIngresosExentos { get; set; }

    [Column("debito_fiscal")]
    public decimal DebitoFiscal { get; set; }

    [Column("credito_fiscal")]
    public decimal CreditoFiscal { get; set; }

    [Column("iva_neto")]
    public decimal IvaNeto { get; set; }

    [Column("saldo_favor_anterior")]
    public decimal SaldoFavorAnterior { get; set; }

    [Column("saldo_favor_resultante")]
    public decimal SaldoFavorResultante { get; set; }

    [Column("renta_bruta")]
    public decimal? RentaBruta { get; set; }

    [Column("deduccion_aplicada")]
    public decimal? DeduccionAplicada { get; set; }

    [Column("renta_neta")]
    public decimal? RentaNeta { get; set; }

    [Column("impuesto_renta")]
    public decimal? ImpuestoRenta { get; set; }

    [Column("retenciones_acreditadas")]
    public decimal? RetencionesAcreditadas { get; set; }

    [Column("monto_total_a_pagar")]
    public decimal MontoTotalAPagar { get; set; }

    [Column("detalle_calculo", TypeName = "jsonb")]
    public string DetalleCalculo { get; set; } = "{}";

    [Column("borrador_generado")]
    public bool BorradorGenerado { get; set; }

    [Column("fecha_generacion_borrador")]
    public DateTimeOffset? FechaGeneracionBorrador { get; set; }

    public ICollection<Exportacion> Exportaciones { get; set; } = new List<Exportacion>();
    public ICollection<ObligacionTributaria> Obligaciones { get; set; } = new List<ObligacionTributaria>();
}
