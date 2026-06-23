using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Requests;

public class ReclasificacionIngresoRequest
{
    [Required]
    public string UsuarioId { get; set; } = string.Empty;

    [Required]
    public ClasificacionIva ClasificacionIva { get; set; }

    [Required]
    public string Justificacion { get; set; } = string.Empty;
}
