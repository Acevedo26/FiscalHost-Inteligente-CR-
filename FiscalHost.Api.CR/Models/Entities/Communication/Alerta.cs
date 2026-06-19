using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.Entities.Communication;

[Table("alerta", Schema = "fiscalhost_db")]
public class Alerta
{
    [Key]
    [Column("alerta_id")]
    public Guid AlertaId { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [Column("obligacion_id")]
    public Guid? ObligacionId { get; set; }
    public ObligacionTributaria? Obligacion { get; set; }

    [Column("tipo_alerta")]
    public TipoAlerta TipoAlerta { get; set; }

    [Column("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [Column("mensaje")]
    public string Mensaje { get; set; } = string.Empty;

    [Column("prioridad")]
    public short Prioridad { get; set; }

    [Column("monto_estimado")]
    public decimal? MontoEstimado { get; set; }

    [Column("canal")]
    public CanalNotificacion Canal { get; set; }

    [Column("estado")]
    public EstadoNotificacion Estado { get; set; }

    [Column("accion_sugerida")]
    public string AccionSugerida { get; set; } = string.Empty;

    [Column("fecha_programada")]
    public DateTimeOffset FechaProgramada { get; set; }

    [Column("fecha_envio")]
    public DateTimeOffset? FechaEnvio { get; set; }

    [Column("fecha_lectura")]
    public DateTimeOffset? FechaLectura { get; set; }

    [Column("intentos_envio")]
    public short IntentosEnvio { get; set; }
}
