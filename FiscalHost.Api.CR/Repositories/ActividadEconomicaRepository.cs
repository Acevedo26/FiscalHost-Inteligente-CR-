using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Repositories;

public interface IActividadEconomicaRepository
{
    Task<ActividadEconomica?> GetByCodigoAsync(string codigo);
    Task<IEnumerable<ActividadEconomica>> GetAllActivasAsync();
}

public class ActividadEconomicaRepository(AppDbContext db) : IActividadEconomicaRepository
{
    public Task<ActividadEconomica?> GetByCodigoAsync(string codigo) =>
        db.ActividadesEconomicas.FirstOrDefaultAsync(a => a.Codigo == codigo && a.Activa);

    public async Task<IEnumerable<ActividadEconomica>> GetAllActivasAsync() =>
        await db.ActividadesEconomicas.Where(a => a.Activa).ToListAsync();
}
