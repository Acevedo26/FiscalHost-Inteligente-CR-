using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Requests;

// ========================================================================
// DTO de Eliminación de Gasto: Obliga a incluir justificación (Ley 8968)
// ========================================================================
public class DeleteGastoRequest
{
    // Requerido para auditar por qué se eliminó el gasto.
    [Required(ErrorMessage = "La justificación es obligatoria según la Ley 8968.")]
    [MinLength(10, ErrorMessage = "La justificación debe detallar el motivo de la eliminación.")]
    public string Justificacion { get; set; } = string.Empty;
}
