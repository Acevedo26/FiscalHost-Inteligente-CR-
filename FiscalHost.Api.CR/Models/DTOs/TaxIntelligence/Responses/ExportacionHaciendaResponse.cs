namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

public class ExportacionHaciendaResponse
{
    public bool Success { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public Guid? ExportacionId { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public string Formato { get; set; } = string.Empty;
    public string TipoContenido { get; set; } = string.Empty;
    public string TipoMime { get; set; } = string.Empty;
    public long TamanioBytes { get; set; }
    public bool EstaProtegido { get; set; }
    public string ContenidoBase64 { get; set; } = string.Empty;
}
