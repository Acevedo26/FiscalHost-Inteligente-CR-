using FiscalHost.Api.CR.Models.Enums.TaxIntelligence;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

/// <summary>
/// Representa un período fiscal faltante detectado.
/// </summary>
public class PeriodoFaltanteDto
{
    public short Anio { get; set; }
    public short Mes { get; set; }
    public TipoFormulario TipoFormulario { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}

/// <summary>
/// Respuesta para la detección de períodos faltantes.
/// </summary>
public class DetectarPeriodosFaltantesResponse
{
    public Guid UsuarioId { get; set; }
    public short AnioInicio { get; set; }
    public short AnioFin { get; set; }
    public TipoFormulario TipoFormulario { get; set; }
    public int TotalPeriodosEsperados { get; set; }
    public int TotalPeriodosExistentes { get; set; }
    public int TotalPeriodosFaltantes { get; set; }
    public List<PeriodoFaltanteDto> PeriodosFaltantes { get; set; } = [];
}