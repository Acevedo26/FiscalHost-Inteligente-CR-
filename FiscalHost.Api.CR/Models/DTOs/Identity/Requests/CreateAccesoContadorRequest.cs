using System;
using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Requests;

// ========================================================================
// DTO de Request (Entrada): Contiene, únicamente, los datos que el usuario
// o frontend envía a la API al realizar una petición, sin incluir
// identificadores generados ni campos de auditoría internos.
// ========================================================================


public class CreateAccesoContadorRequest
{
    [Required]
    [EmailAddress]
    public string CorreoContador { get; set; } = string.Empty;

    [Required]
    public string Permisos { get; set; } = "{}";
}


