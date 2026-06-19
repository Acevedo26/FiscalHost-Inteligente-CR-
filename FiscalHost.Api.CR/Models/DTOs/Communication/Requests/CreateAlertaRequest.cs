using System;
using System.ComponentModel.DataAnnotations;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Communication.Requests;

public class CreateAlertaRequest
{
    public Guid? ObligacionId { get; set; }

    [Required]
    public TipoAlerta TipoAlerta { get; set; }

    [Required]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    public string Mensaje { get; set; } = string.Empty;

    [Required]
    public short Prioridad { get; set; }

    public decimal? MontoEstimado { get; set; }

    [Required]
    public CanalNotificacion Canal { get; set; }

    [Required]
    public string AccionSugerida { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset FechaProgramada { get; set; }
}
