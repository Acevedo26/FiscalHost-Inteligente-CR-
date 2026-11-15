using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

namespace FiscalHost.Api.CR.Services;

/// <summary>
/// Interfaz para el servicio de reconstrucción histórica retroactiva.
/// </summary>
public interface IReconstruccionHistoricaService
{
    Task<ReconstruccionHistoricaResponse> ReconstruirAsync(Guid usuarioId, ReconstruccionHistoricaRequest request);
    Task<DetectarPeriodosFaltantesResponse> DetectarPeriodosFaltantesAsync(Guid usuarioId, DetectarPeriodosFaltantesRequest request);
    Task<string> ObtenerReporteErroresAsync(Guid reconstruccionId);
}