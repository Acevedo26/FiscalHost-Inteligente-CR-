using System;
using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Requests;

public class CreateAccesoContadorRequest
{
    [Required]
    [EmailAddress]
    public string CorreoContador { get; set; } = string.Empty;

    [Required]
    public string Permisos { get; set; } = "{}";
}
