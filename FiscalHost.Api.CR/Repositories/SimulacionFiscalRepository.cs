using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Repositories;

public class SimulacionFiscalRepository : ISimulacionFiscalRepository
{
    private readonly AppDbContext _context;

    public SimulacionFiscalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SimulacionFiscal> CreateAsync(SimulacionFiscal simulacion)
    {
        _context.SimulacionesFiscales.Add(simulacion);
        await _context.SaveChangesAsync();
        return simulacion;
    }

    public async Task<SimulacionFiscal?> GetByIdAsync(Guid simulacionId, Guid usuarioId)
    {
        return await _context.SimulacionesFiscales
            .FirstOrDefaultAsync(s => s.SimulacionId == simulacionId && s.UsuarioId == usuarioId);
    }

    public async Task<IEnumerable<SimulacionFiscal>> GetAllByUsuarioIdAsync(Guid usuarioId)
    {
        return await _context.SimulacionesFiscales
            .Where(s => s.UsuarioId == usuarioId)
            .OrderByDescending(s => s.PeriodoBaseAnio)
            .ThenByDescending(s => s.PeriodoBaseMes)
            .ToListAsync();
    }

    public async Task DeleteAsync(SimulacionFiscal simulacion)
    {
        _context.SimulacionesFiscales.Remove(simulacion);
        await _context.SaveChangesAsync();
    }
}
