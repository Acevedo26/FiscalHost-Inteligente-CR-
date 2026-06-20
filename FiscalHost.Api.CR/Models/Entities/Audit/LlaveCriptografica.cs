using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiscalHost.Api.CR.Models.Entities.Audit;

// ========================================================================
// Entidad: Representa una tabla en la base de datos PostgreSQL, la cual se
// utiliza exclusivamente por Entity Framework para operaciones de lectura
// y escritura.
// ========================================================================


public class LlaveCriptografica
{
    [Key]
    [Column("llave_id")]
    public Guid LlaveId { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    [Column("nombre_archivo")]
    public string NombreArchivo { get; set; } = string.Empty;

    [Column("ruta_blob_storage")]
    public string RutaBlobStorage { get; set; } = string.Empty;

    [Column("hash_integridad")]
    public string HashIntegridad { get; set; } = string.Empty;

    [Column("referencia_key_vault")]
    public string? ReferenciaKeyVault { get; set; }

    [Column("huella_digital_certificado")]
    public string? HuellaDigitalCertificado { get; set; }

    [Column("fecha_emision_certificado")]
    public DateTime? FechaEmisionCertificado { get; set; }

    [Column("fecha_expiracion_certificado")]
    public DateTime? FechaExpiracionCertificado { get; set; }

    [Column("emisor_certificado")]
    public string? EmisorCertificado { get; set; }

    [Column("estado")]
    public string Estado { get; set; } = "ACTIVA";

    [Column("fecha_carga")]
    public DateTime FechaCarga { get; set; } = DateTime.UtcNow;

    [Column("ultima_actualizacion_contrasena")]
    public DateTime? UltimaActualizacionContrasena { get; set; }

    public ICollection<AuditoriaLlave> Auditorias { get; set; } = [];

    // --- Backwards Compatibility Properties ---
    [NotMapped]
    public int Id { get => LlaveId.GetHashCode(); set { } }
    
    [NotMapped]
    public string AnfitrionId { get => UsuarioId.ToString(); set => UsuarioId = Guid.TryParse(value, out var g) ? g : Guid.Empty; }
    
    [NotMapped]
    public byte[] ContenidoCifrado { get; set; } = [];
    
    [NotMapped]
    public string ContrasenaHash { get; set; } = string.Empty;
    
    [NotMapped]
    public DateTime FechaActualizacion { get => UltimaActualizacionContrasena ?? FechaCarga; set => UltimaActualizacionContrasena = value; }
    
    [NotMapped]
    public bool Activa { get => Estado == "ACTIVA"; set => Estado = value ? "ACTIVA" : "INACTIVA"; }
}


