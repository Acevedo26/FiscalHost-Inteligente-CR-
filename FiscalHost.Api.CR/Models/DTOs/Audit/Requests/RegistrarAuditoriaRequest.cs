using System.ComponentModel.DataAnnotations;
using FiscalHost.Api.CR.Models.Enums.Audit;
using FiscalHost.Api.CR.Models.Enums.Identity;

namespace FiscalHost.Api.CR.Models.DTOs.Audit.Requests;

public class RegistrarAuditoriaRequest
{
    public Guid? UsuarioId { get; set; }

    [EmailAddress]
    public string? CorreoUsuario { get; set; }

    public RolUsuario? RolUsuario { get; set; }

    [Required]
    public OperacionAuditoria Operacion { get; set; }

    [Required]
    public string TablaAfectada { get; set; } = string.Empty;

    public Guid? RegistroId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string[]? CamposModificados { get; set; }

    public bool EsCampoSensible { get; set; }

    public string? Justificacion { get; set; }
}
