using System;
using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Communication.Requests;

public class CreateContenidoEducativoRequest
{
    [Required]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    public string Slug { get; set; } = string.Empty;

    [Required]
    public string Categoria { get; set; } = string.Empty;

    [Required]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    public string ContenidoMarkdown { get; set; } = string.Empty;

    public bool EsTutorialPrimerUso { get; set; }

    public int OrdenDisplay { get; set; }
    
    public bool Publicado { get; set; }
}
