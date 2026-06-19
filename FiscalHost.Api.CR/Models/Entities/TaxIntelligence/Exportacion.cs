using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiscalHost.Api.CR.Models.Entities.TaxIntelligence;

[Table("exportacion", Schema = "fiscalhost_db")]
public class Exportacion
{
    [Key]
    [Column("exportacion_id")]
    public Guid ExportacionId { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [Column("calculo_id")]
    public Guid? CalculoId { get; set; }
    public CalculoFiscal? CalculoFiscal { get; set; }

    [Column("formato")]
    public string Formato { get; set; } = string.Empty;

    [Column("tipo_contenido")]
    public string TipoContenido { get; set; } = string.Empty;

    [Column("archivo_url")]
    public string ArchivoUrl { get; set; } = string.Empty;

    [Column("nombre_archivo")]
    public string NombreArchivo { get; set; } = string.Empty;

    [Column("tamanio_bytes")]
    public long? TamanioBytes { get; set; }

    [Column("esta_protegido")]
    public bool EstaProtegido { get; set; }

    [Column("expira_at")]
    public DateTimeOffset? ExpiraAt { get; set; }
}
