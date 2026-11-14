using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Repositories;

public class ReconstruccionHistoricaRepository(AppDbContext db) : IReconstruccionHistoricaRepository
{
    public async Task AddAsync(ReconstruccionHistorica reconstruccion)
    {
        await db.ReconstruccionesHistoricas.AddAsync(reconstruccion);
    }

    public async Task<ReconstruccionHistorica?> GetByIdAsync(Guid reconstruccionId)
    {
        return await db.ReconstruccionesHistoricas
            .FirstOrDefaultAsync(r => r.ReconstruccionId == reconstruccionId);
    }

    public Task UpdateAsync(ReconstruccionHistorica reconstruccion)
    {
        db.ReconstruccionesHistoricas.Update(reconstruccion);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}