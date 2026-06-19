using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiscalHost.Api.CR.Models.Entities.Communication;

[Table("contenido_educativo", Schema = "fiscalhost_db")]
public class ContenidoEducativo
{
    [Key]
    [Column("contenido_id")]
    public Guid ContenidoId { get; set; }

    [Column("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [Column("slug")]
    public string Slug { get; set; } = string.Empty;

    [Column("categoria")]
    public string Categoria { get; set; } = string.Empty;

    [Column("tipo")]
    public string Tipo { get; set; } = string.Empty;

    [Column("contenido_markdown")]
    public string ContenidoMarkdown { get; set; } = string.Empty;

    [Column("contenido_html")]
    public string? ContenidoHtml { get; set; }

    [Column("es_tutorial_primer_uso")]
    public bool EsTutorialPrimerUso { get; set; }

    [Column("orden_display")]
    public int OrdenDisplay { get; set; }

    [Column("version")]
    public int Version { get; set; }

    [Column("publicado")]
    public bool Publicado { get; set; }

    [Column("autor_id")]
    public Guid? AutorId { get; set; }
    public Usuario? Autor { get; set; }

    [Column("published_at")]
    public DateTimeOffset? PublishedAt { get; set; }
}
