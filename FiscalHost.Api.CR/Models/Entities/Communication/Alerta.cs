using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.Entities.Communication;

// ========================================================================
// Entidad: Representa una tabla en la base de datos PostgreSQL, la cual se
// utiliza exclusivamente por Entity Framework para operaciones de lectura
// y escritura.
// ========================================================================


[Table("alerta", Schema = "fiscalhost_db")]
public class Alerta
{
    [Key]
    [Required]
    [Column("alerta_id")]
    public Guid AlertaId { get; set; }

    [Required]
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [Column("obligacion_id")]
    public Guid? ObligacionId { get; set; }
    public ObligacionTributaria? Obligacion { get; set; }

    [Required]
    [Column("tipo_alerta")]
    public TipoAlerta TipoAlerta { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [Column("mensaje")]
    public string Mensaje { get; set; } = string.Empty;

    [Required]
    [Column("prioridad")]
    public short Prioridad { get; set; }

    [Column("monto_estimado")]
    public decimal? MontoEstimado { get; set; }

    [Required]
    [Column("canal")]
    public CanalNotificacion Canal { get; set; }

    [Required]
    [Column("estado")]
    public EstadoNotificacion Estado { get; set; }

    [Column("accion_sugerida", TypeName = "jsonb")]
    public string AccionSugerida { get; set; } = "{}";

    [Required]
    [Column("fecha_programada")]
    public DateTimeOffset FechaProgramada { get; set; }

    [Column("fecha_envio")]
    public DateTimeOffset? FechaEnvio { get; set; }

    [Column("fecha_lectura")]
    public DateTimeOffset? FechaLectura { get; set; }

    [Column("error_envio")]
    public string? ErrorEnvio { get; set; }

    [Required]
    [Column("intentos_envio")]
    public short IntentosEnvio { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}


