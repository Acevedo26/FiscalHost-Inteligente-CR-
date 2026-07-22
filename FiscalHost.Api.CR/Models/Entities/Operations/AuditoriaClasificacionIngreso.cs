using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiscalHost.Api.CR.Models.Entities.Operations;

[Table("auditoria_clasificacion_ingreso", Schema = "fiscalhost_db")]
public class AuditoriaClasificacionIngreso
{
    [Key]
    [Column("auditoria_clasificacion_ingreso_id")]
    public int Id { get; set; }

    [Column("clasificacion_ingreso_id")]
    public int ClasificacionIngresoId { get; set; }

    public ClasificacionIngreso ClasificacionIngreso { get; set; } = null!;

    [Column("usuario_id")]
    public string UsuarioId { get; set; } = string.Empty;

    [Column("valor_anterior")]
    public string ValorAnterior { get; set; } = string.Empty;

    [Column("valor_nuevo")]
    public string ValorNuevo { get; set; } = string.Empty;

    [Column("justificacion")]
    public string Justificacion { get; set; } = string.Empty;

    [Column("fecha_evento")]
    public DateTime FechaEvento { get; set; } = DateTime.UtcNow;
}
