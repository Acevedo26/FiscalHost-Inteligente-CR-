using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;

namespace FiscalHost.Api.CR.Repositories;

public interface ISimulacionFiscalRepository
{
    Task<SimulacionFiscal> CreateAsync(SimulacionFiscal simulacion);
    Task<SimulacionFiscal?> GetByIdAsync(Guid simulacionId, Guid usuarioId);
    Task<IEnumerable<SimulacionFiscal>> GetAllByUsuarioIdAsync(Guid usuarioId);
    Task DeleteAsync(SimulacionFiscal simulacion);
}
