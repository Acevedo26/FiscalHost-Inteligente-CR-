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


[Table("obligacion_tributaria", Schema = "fiscalhost_db")]
public class ObligacionTributaria
{
    [Key]
    [Required]
    [Column("obligacion_id")]
    public Guid ObligacionId { get; set; }

    [Required]
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [Column("calculo_id")]
    public Guid? CalculoId { get; set; }
    
    [ForeignKey(nameof(CalculoId))]
    public CalculoFiscal? CalculoFiscal { get; set; }

    [Required]
    [Column("periodo_id")]
    public Guid PeriodoId { get; set; }
    public PeriodoFiscal Periodo { get; set; } = null!;

    [Required]
    [Column("tipo_formulario")]
    public TipoFormulario TipoFormulario { get; set; }

    [Required]
    [MaxLength(300)]
    [Column("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    [Required]
    [Column("monto_capital")]
    public decimal MontoCapital { get; set; }

    [Required]
    [Column("monto_multa")]
    public decimal MontoMulta { get; set; }

    [Required]
    [Column("monto_intereses_acumulados")]
    public decimal MontoInteresesAcumulados { get; set; }

    [Required]
    [Column("monto_total_actualizado")]
    public decimal MontoTotalActualizado { get; set; }

    [Required]
    [Column("fecha_vencimiento", TypeName = "date")]
    public DateOnly FechaVencimiento { get; set; }

    [Column("fecha_pago", TypeName = "date")]
    public DateOnly? FechaPago { get; set; }

    [Required]
    [Column("estado")]
    public EstadoObligacion Estado { get; set; }

    [Column("tasa_interes_aplicada")]
    public decimal? TasaInteresAplicada { get; set; }

    [Column("fecha_ultimo_calculo_interes")]
    public DateTimeOffset? FechaUltimoCalculoInteres { get; set; }

    [Column("historial_intereses", TypeName = "jsonb")]
    public string HistorialIntereses { get; set; } = "{}";

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
    public ICollection<SancionAutoliquidacion> Sanciones { get; set; } = new List<SancionAutoliquidacion>();
}


