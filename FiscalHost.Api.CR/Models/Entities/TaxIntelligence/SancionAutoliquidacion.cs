using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiscalHost.Api.CR.Models.Entities.TaxIntelligence;

[Table("sancion_autoliquidacion", Schema = "fiscalhost_db")]
public class SancionAutoliquidacion
{
    [Key]
    [Column("sancion_id")]
    public Guid SancionId { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [Column("obligacion_id")]
    public Guid? ObligacionId { get; set; }
    public ObligacionTributaria? Obligacion { get; set; }

    [Column("tipo_sancion")]
    public string TipoSancion { get; set; } = string.Empty;

    [Column("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    [Column("fecha_omision")]
    public DateTime FechaOmision { get; set; }

    [Column("monto_base_adeudado")]
    public decimal MontoBaseAdeudado { get; set; }

    [Column("multa_base_calculada")]
    public decimal MultaBaseCalculada { get; set; }

    [Column("porcentaje_reduccion")]
    public decimal PorcentajeReduccion { get; set; }

    [Column("monto_reduccion")]
    public decimal MontoReduccion { get; set; }

    [Column("multa_reducida")]
    public decimal MultaReducida { get; set; }

    [Column("intereses_acumulados")]
    public decimal InteresesAcumulados { get; set; }

    [Column("monto_total_pagar")]
    public decimal MontoTotalPagar { get; set; }

    [Column("detalle_calculo", TypeName = "jsonb")]
    public string DetalleCalculo { get; set; } = "{}";

    [Column("estado")]
    public string Estado { get; set; } = string.Empty;
}
