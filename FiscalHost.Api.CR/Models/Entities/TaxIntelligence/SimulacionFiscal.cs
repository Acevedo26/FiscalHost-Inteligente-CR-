using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiscalHost.Api.CR.Models.Entities.TaxIntelligence;

[Table("simulacion_fiscal", Schema = "fiscalhost_db")]
public class SimulacionFiscal
{
    [Key]
    [Column("simulacion_id")]
    public Guid SimulacionId { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [Column("periodo_base_anio")]
    public short PeriodoBaseAnio { get; set; }

    [Column("periodo_base_mes")]
    public short? PeriodoBaseMes { get; set; }

    [Column("parametros_entrada", TypeName = "jsonb")]
    public string ParametrosEntrada { get; set; } = "{}";

    [Column("resultados", TypeName = "jsonb")]
    public string Resultados { get; set; } = "{}";
}
