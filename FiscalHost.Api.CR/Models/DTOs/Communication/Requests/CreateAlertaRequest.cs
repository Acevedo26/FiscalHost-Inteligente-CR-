using System;
using System.ComponentModel.DataAnnotations;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Communication.Requests;

// ========================================================================
// DTO de Request (Entrada): Contiene, únicamente, los datos que el usuario
// o frontend envía a la API al realizar una petición, sin incluir
// identificadores generados ni campos de auditoría internos.
// ========================================================================


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

