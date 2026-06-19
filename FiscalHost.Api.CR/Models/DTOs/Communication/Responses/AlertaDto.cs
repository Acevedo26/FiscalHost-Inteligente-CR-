using System;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Communication.Responses;

public class AlertaDto
{
    public Guid AlertaId { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid? ObligacionId { get; set; }
    public TipoAlerta TipoAlerta { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public short Prioridad { get; set; }
    public decimal? MontoEstimado { get; set; }
    public CanalNotificacion Canal { get; set; }
    public EstadoNotificacion Estado { get; set; }
    public string AccionSugerida { get; set; } = string.Empty;
    public DateTimeOffset FechaProgramada { get; set; }
    public DateTimeOffset? FechaEnvio { get; set; }
    public DateTimeOffset? FechaLectura { get; set; }
    public short IntentosEnvio { get; set; }
}
