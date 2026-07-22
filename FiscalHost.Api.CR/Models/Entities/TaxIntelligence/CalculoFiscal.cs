using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.Entities.TaxIntelligence;

// ========================================================================
// Entidad: Representa una tabla en la base de datos PostgreSQL, la cual se
// utiliza exclusivamente por Entity Framework para operaciones de lectura
// y escritura.
// ========================================================================


[Table("calculo_fiscal", Schema = "fiscalhost_db")]
public class CalculoFiscal
{
    [Key]
    [Required]
    [Column("calculo_id")]
    public Guid CalculoId { get; set; }

    [Required]
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [Required]
    [Column("periodo_id")]
    public Guid PeriodoId { get; set; }
    public PeriodoFiscal Periodo { get; set; } = null!;

    [Required]
    [Column("tipo_formulario")]
    public TipoFormulario TipoFormulario { get; set; }

    [Column("regimen_aplicado")]
    public RegimenTributario? RegimenAplicado { get; set; }

    [Required]
    [Column("estado")]
    public EstadoDeclaracion Estado { get; set; }

    [Required]
    [Column("total_ingresos_brutos")]
    public decimal TotalIngresosBrutos { get; set; }

    [Required]
    [Column("total_ingresos_gravados")]
    public decimal TotalIngresosGravados { get; set; }

    [Required]
    [Column("total_ingresos_exentos")]
    public decimal TotalIngresosExentos { get; set; }

    [Required]
    [Column("debito_fiscal")]
    public decimal DebitoFiscal { get; set; }

    [Required]
    [Column("credito_fiscal")]
    public decimal CreditoFiscal { get; set; }

    [Required]
    [Column("iva_neto")]
    public decimal IvaNeto { get; set; }

    [Required]
    [Column("saldo_favor_anterior")]
    public decimal SaldoFavorAnterior { get; set; }

    [Required]
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

    [Required]
    [Column("monto_total_a_pagar")]
    public decimal MontoTotalAPagar { get; set; }

    [Required]
    [Column("detalle_calculo", TypeName = "jsonb")]
    public string DetalleCalculo { get; set; } = "{}";

    [Required]
    [Column("borrador_generado")]
    public bool BorradorGenerado { get; set; }

    [Column("fecha_generacion_borrador")]
    public DateTimeOffset? FechaGeneracionBorrador { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Exportacion> Exportaciones { get; set; } = new List<Exportacion>();
    public ICollection<ObligacionTributaria> Obligaciones { get; set; } = new List<ObligacionTributaria>();
}


