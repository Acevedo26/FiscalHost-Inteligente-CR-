using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;

namespace FiscalHost.Api.CR.Repositories;

/// <summary>
/// Interfaz para el repositorio de reconstrucción histórica.
/// </summary>
public interface IReconstruccionHistoricaRepository
{
    Task AddAsync(ReconstruccionHistorica reconstruccion);
    Task<ReconstruccionHistorica?> GetByIdAsync(Guid reconstruccionId);
    Task UpdateAsync(ReconstruccionHistorica reconstruccion);
    Task SaveChangesAsync();
}