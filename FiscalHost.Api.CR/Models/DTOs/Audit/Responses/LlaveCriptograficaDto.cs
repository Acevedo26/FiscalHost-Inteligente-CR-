using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FiscalHost.Api.CR.Models.DTOs.Audit.Responses;

// ========================================================================
// DTO de Response (Salida): Contiene la información formateada que la API
// le devuelve al frontend para mostrar en pantalla, ocultando, de esta
// manera, cualquier dato sensible.
// ========================================================================


public class CargarLlaveRequest
{
    [Required] public string AnfitrionId { get; set; } = string.Empty;
    [Required] public IFormFile Archivo { get; set; } = null!;
    [Required] public string Contrasena { get; set; } = string.Empty;
}

public class ActualizarContrasenaRequest
{
    [Required] public string AnfitrionId { get; set; } = string.Empty;
    [Required] public string ContrasenaActual { get; set; } = string.Empty;
    [Required] public string ContrasenaNueva { get; set; } = string.Empty;
}

public class LlaveCriptograficaResponse
{
    public int Id { get; set; }
    public string AnfitrionId { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public DateTime FechaActualizacion { get; set; }
    public bool Activa { get; set; }
}

public class LlaveCriptograficaDto : LlaveCriptograficaResponse
{
}

