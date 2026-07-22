using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Repositories;

public interface IExportacionHaciendaRepository
{
    Task<List<Reserva>> GetReservasAsync(Guid usuarioId, short anioFiscal, short? mes);
    Task<List<Gasto>> GetGastosAsync(Guid usuarioId, short anioFiscal, short? mes);
    Task<CalculoFiscal?> GetCalculoAsync(Guid usuarioId, Guid calculoId);
    Task AddExportacionAsync(Exportacion exportacion);
    Task SaveChangesAsync();
}

public class ExportacionHaciendaRepository(AppDbContext db)
    : IExportacionHaciendaRepository
{
    public Task<List<Reserva>> GetReservasAsync(Guid usuarioId, short anioFiscal, short? mes)
    {
        var query = db.Reservas
            .Where(r => r.UsuarioId == usuarioId && r.PeriodoFiscalAnio == anioFiscal);

        if (mes.HasValue)
            query = query.Where(r => r.PeriodoFiscalMes == mes.Value);

        return query.ToListAsync();
    }

    public Task<List<Gasto>> GetGastosAsync(Guid usuarioId, short anioFiscal, short? mes)
    {
        var query = db.Gastos
            .Where(g => g.UsuarioId == usuarioId && g.PeriodoFiscalAnio == anioFiscal);

        if (mes.HasValue)
            query = query.Where(g => g.PeriodoFiscalMes == mes.Value);

        return query.ToListAsync();
    }

    public Task<CalculoFiscal?> GetCalculoAsync(Guid usuarioId, Guid calculoId)
    {
        return db.CalculosFiscales
            .Include(c => c.Periodo)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.CalculoId == calculoId);
    }

    public async Task AddExportacionAsync(Exportacion exportacion)
    {
        await db.Exportaciones.AddAsync(exportacion);
    }

    public Task SaveChangesAsync()
    {
        return db.SaveChangesAsync();
    }
}
