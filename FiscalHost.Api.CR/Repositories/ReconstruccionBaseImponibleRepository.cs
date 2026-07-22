using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Repositories;

public interface IReconstruccionBaseImponibleRepository
{
    Task<List<Reserva>> GetReservasPorAnioAsync(Guid usuarioId, short anioFiscal);
    Task<List<PeriodoFiscal>> GetPeriodosPorAnioAsync(short anioFiscal);
    Task AddCalculosAsync(List<CalculoFiscal> calculos);
    Task SaveChangesAsync();
}

public class ReconstruccionBaseImponibleRepository(AppDbContext db)
    : IReconstruccionBaseImponibleRepository
{
    public Task<List<Reserva>> GetReservasPorAnioAsync(Guid usuarioId, short anioFiscal)
    {
        return db.Reservas
            .Where(r => r.UsuarioId == usuarioId && r.PeriodoFiscalAnio == anioFiscal)
            .ToListAsync();
    }

    public Task<List<PeriodoFiscal>> GetPeriodosPorAnioAsync(short anioFiscal)
    {
        return db.PeriodosFiscales
            .Where(p => p.Anio == anioFiscal)
            .ToListAsync();
    }

    public async Task AddCalculosAsync(List<CalculoFiscal> calculos)
    {
        await db.CalculosFiscales.AddRangeAsync(calculos);
    }

    public Task SaveChangesAsync()
    {
        return db.SaveChangesAsync();
    }
}
