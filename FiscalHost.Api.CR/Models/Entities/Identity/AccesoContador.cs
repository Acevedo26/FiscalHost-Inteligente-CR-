using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiscalHost.Api.CR.Models.Entities.Identity;

// ========================================================================
// Entidad: Representa una tabla en la base de datos PostgreSQL, la cual se
// utiliza exclusivamente por Entity Framework para operaciones de lectura
// y escritura.
// ========================================================================


[Table("acceso_contador", Schema = "fiscalhost_db")]
public class AccesoContador
{
    [Key]
    [Column("acceso_id")]
    public Guid AccesoId { get; set; }

    [Column("anfitrion_id")]
    public Guid AnfitrionId { get; set; }
    public Usuario Anfitrion { get; set; } = null!;

    [Column("contador_id")]
    public Guid? ContadorId { get; set; }
    public Usuario? Contador { get; set; }

    [Column("correo_contador")]
    public string CorreoContador { get; set; } = string.Empty;

    [Column("permisos", TypeName = "jsonb")]
    public string Permisos { get; set; } = "{}";

    [Column("fecha_invitacion")]
    public DateTimeOffset FechaInvitacion { get; set; }

    [Column("fecha_aceptacion")]
    public DateTimeOffset? FechaAceptacion { get; set; }

    [Column("fecha_expiracion")]
    public DateTimeOffset? FechaExpiracion { get; set; }

    [Column("fecha_revocacion")]
    public DateTimeOffset? FechaRevocacion { get; set; }

    [Column("estado")]
    public string Estado { get; set; } = string.Empty;
}


