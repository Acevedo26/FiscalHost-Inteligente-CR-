using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FiscalHost.Api.CR.Models.Enums.TaxIntelligence;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

/// <summary>
/// Request para detectar períodos fiscales faltantes en un rango de años.
/// </summary>
public class DetectarPeriodosFaltantesRequest
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }

    [Required]
    [Range(2020, 2026)]
    public short AnioInicio { get; set; }

    [Required]
    [Range(2020, 2026)]
    public short AnioFin { get; set; }

    [Required]
    public TipoFormulario TipoFormulario { get; set; }
}