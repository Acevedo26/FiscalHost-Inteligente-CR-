using System.Text.Json.Serialization;
using FiscalHost.Api.CR.Models.Enums.TaxIntelligence;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

/// <summary>
/// Resultado de una fila procesada en la reconstrucción histórica.
/// </summary>
public class ReconstruccionHistoricaFilaResultado
{
    public short Anio { get; set; }
    public short Mes { get; set; }
    public TipoFormulario TipoFormulario { get; set; }
    public bool EsValida { get; set; }
    public string? Error { get; set; }
    public Guid? CalculoId { get; set; }
    public Guid? ObligacionId { get; set; }
    public decimal? IvaCalculado { get; set; }
    public decimal? RentaCapitalCalculada { get; set; }
    public decimal? MultaInscripcionTardia { get; set; }
    public decimal? MontoTotalObligacion { get; set; }
}

/// <summary>
/// Respuesta completa de la reconstrucción histórica.
/// </summary>
public class ReconstruccionHistoricaResponse
{
    public Guid ReconstruccionId { get; set; }
    public Guid UsuarioId { get; set; }
    public int TotalFilas { get; set; }
    public int FilasExitosas { get; set; }
    public int FilasConError { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? ReporteErroresUrl { get; set; }
    public List<ReconstruccionHistoricaFilaResultado> Resultados { get; set; } = [];
}