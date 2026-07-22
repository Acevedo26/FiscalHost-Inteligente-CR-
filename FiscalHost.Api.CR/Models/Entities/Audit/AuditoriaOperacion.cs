using FiscalHost.Api.CR.Models.Enums.Audit;
using FiscalHost.Api.CR.Models.Enums.Identity;

namespace FiscalHost.Api.CR.Models.Entities.Audit;

public class AuditoriaOperacion
{
    public Guid AuditId { get; set; } = Guid.NewGuid();

    // Quién
    public Guid? UsuarioId { get; set; }
    public string? CorreoUsuario { get; set; }
    public RolUsuario? RolUsuario { get; set; }

    // Qué
    public OperacionAuditoria Operacion { get; set; }
    public string TablaAfectada { get; set; } = string.Empty;
    public Guid? RegistroId { get; set; }

    // Valores
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string[]? CamposModificados { get; set; }

    // Justificación (Ley 8968)
    public string? Justificacion { get; set; }

    // Contexto
    public System.Net.IPAddress? IpOrigen { get; set; }
    public string? UserAgent { get; set; }
    public Guid? RequestId { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
