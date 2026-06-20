using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiscalHost.Api.CR.Models.Entities.TaxIntelligence;

// ========================================================================
// Entidad: Representa una tabla en la base de datos PostgreSQL, la cual se
// utiliza exclusivamente por Entity Framework para operaciones de lectura
// y escritura.
// ========================================================================


[Table("exportacion", Schema = "fiscalhost_db")]
public class Exportacion
{
    [Key]
    [Required]
    [Column("exportacion_id")]
    public Guid ExportacionId { get; set; }

    [Required]
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [Column("calculo_id")]
    public Guid? CalculoId { get; set; }
    public CalculoFiscal? CalculoFiscal { get; set; }

    [Required]
    [MaxLength(10)]
    [Column("formato")]
    public string Formato { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("tipo_contenido")]
    public string TipoContenido { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    [Column("archivo_url")]
    public string ArchivoUrl { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("nombre_archivo")]
    public string NombreArchivo { get; set; } = string.Empty;

    [Column("tamanio_bytes")]
    public long? TamanioBytes { get; set; }

    [Required]
    [Column("esta_protegido")]
    public bool EstaProtegido { get; set; }

    [Column("expira_at")]
    public DateTimeOffset? ExpiraAt { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}


