using System;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

public class ExportacionDto
{
    public Guid ExportacionId { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid? CalculoId { get; set; }
    public string Formato { get; set; } = string.Empty;
    public string TipoContenido { get; set; } = string.Empty;
    public string ArchivoUrl { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public long? TamanioBytes { get; set; }
    public bool EstaProtegido { get; set; }
    public DateTimeOffset? ExpiraAt { get; set; }
}
