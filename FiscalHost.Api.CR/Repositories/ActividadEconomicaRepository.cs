using FiscalHost.Api.CR.Data;

using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Repositories;

public interface IActividadEconomicaRepository
{
    Task<CatalogoActividadEconomica?> GetByCodigoAsync(string codigo);
    Task<IEnumerable<CatalogoActividadEconomica>> GetAllActivasAsync();
}

public class ActividadEconomicaRepository(AppDbContext db) : IActividadEconomicaRepository
{
    public Task<CatalogoActividadEconomica?> GetByCodigoAsync(string codigo) =>
        db.CatalogoActividadesEconomicas.FirstOrDefaultAsync(a => a.Codigo == codigo && a.Vigente);

    public async Task<IEnumerable<CatalogoActividadEconomica>> GetAllActivasAsync() =>
        await db.CatalogoActividadesEconomicas.Where(a => a.Vigente).ToListAsync();
}
