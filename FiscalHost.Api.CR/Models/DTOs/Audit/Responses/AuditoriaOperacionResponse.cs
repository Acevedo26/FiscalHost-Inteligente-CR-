namespace FiscalHost.Api.CR.Models.DTOs.Audit.Responses;

public class AuditoriaOperacionResponse
{
    public Guid AuditId { get; set; }
    public Guid? UsuarioId { get; set; }
    public string? CorreoUsuario { get; set; }
    public string? RolUsuario { get; set; }
    public string Operacion { get; set; } = string.Empty;
    public string TablaAfectada { get; set; } = string.Empty;
    public Guid? RegistroId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string[]? CamposModificados { get; set; }
    public string? Justificacion { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class ExportacionAuditoriaResponse
{
    public bool Success { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public string TipoMime { get; set; } = "text/csv";
    public string ContenidoBase64 { get; set; } = string.Empty;
}
