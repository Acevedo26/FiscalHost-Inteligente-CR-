using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.Entities.TaxIntelligence;

[Table("obligacion_tributaria", Schema = "fiscalhost_db")]
public class ObligacionTributaria
{
    [Key]
    [Column("obligacion_id")]
    public Guid ObligacionId { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [Column("calculo_id")]
    public Guid? CalculoId { get; set; }
    public CalculoFiscal? CalculoFiscal { get; set; }

    [Column("periodo_id")]
    public Guid PeriodoId { get; set; }
    public PeriodoFiscal Periodo { get; set; } = null!;

    [Column("tipo_formulario")]
    public TipoFormulario TipoFormulario { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    [Column("monto_capital")]
    public decimal MontoCapital { get; set; }

    [Column("monto_multa")]
    public decimal MontoMulta { get; set; }

    [Column("monto_intereses_acumulados")]
    public decimal MontoInteresesAcumulados { get; set; }

    [Column("monto_total_actualizado")]
    public decimal MontoTotalActualizado { get; set; }

    [Column("fecha_vencimiento")]
    public DateTime FechaVencimiento { get; set; }

    [Column("fecha_pago")]
    public DateTime? FechaPago { get; set; }

    [Column("estado")]
    public EstadoObligacion Estado { get; set; }

    [Column("tasa_interes_aplicada")]
    public decimal? TasaInteresAplicada { get; set; }

    [Column("historial_intereses", TypeName = "jsonb")]
    public string HistorialIntereses { get; set; } = "{}";

    public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
    public ICollection<SancionAutoliquidacion> Sanciones { get; set; } = new List<SancionAutoliquidacion>();
}
