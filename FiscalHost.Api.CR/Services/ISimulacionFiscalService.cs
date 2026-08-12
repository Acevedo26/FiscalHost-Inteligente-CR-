using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

namespace FiscalHost.Api.CR.Services;

public interface ISimulacionFiscalService
{
    Task<SimulacionFiscalResponseDto> CrearSimulacionAsync(Guid usuarioId, CreateSimulacionFiscalRequest request);
    Task<SimulacionFiscalResponseDto> ObtenerSimulacionAsync(Guid simulacionId, Guid usuarioId);
    Task<IEnumerable<SimulacionFiscalResponseDto>> ListarSimulacionesAsync(Guid usuarioId);
    Task<ComparacionSimulacionesResponseDto> CompararSimulacionesAsync(Guid usuarioId, List<Guid> simulacionIds);
    Task<byte[]> ExportarSimulacionCsvAsync(Guid simulacionId, Guid usuarioId);
    Task EliminarSimulacionAsync(Guid simulacionId, Guid usuarioId);
}
