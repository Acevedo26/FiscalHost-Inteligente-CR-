using System;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Communication.Responses;

public class ContenidoEducativoDto
{
    public Guid ContenidoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string ContenidoMarkdown { get; set; } = string.Empty;
    public string? ContenidoHtml { get; set; }
    public bool EsTutorialPrimerUso { get; set; }
    public int OrdenDisplay { get; set; }
    public int Version { get; set; }
    public bool Publicado { get; set; }
    public Guid? AutorId { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
}
