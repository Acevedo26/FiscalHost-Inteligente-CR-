using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums.TaxIntelligence;

namespace FiscalHost.Api.CR.Repositories;

public class ObligacionTributariaRepository(AppDbContext dbContext) : IObligacionTributariaRepository
{
    public async Task<ObligacionTributaria?> GetByIdAsync(Guid id)
    {
        return await dbContext.ObligacionesTributarias
            .Include(o => o.Usuario)
            .FirstOrDefaultAsync(o => o.ObligacionId == id);
    }

    public async Task<IEnumerable<ObligacionTributaria>> GetVencidasPendientesAsync(DateOnly fechaCorte)
    {
        return await dbContext.ObligacionesTributarias
            .Include(o => o.Usuario)
            .Where(o => (o.Estado == EstadoObligacion.VIGENTE || o.Estado == EstadoObligacion.VENCIDA) && o.FechaVencimiento < fechaCorte)
            .ToListAsync();
    }

    public Task UpdateAsync(ObligacionTributaria obligacion)
    {
        dbContext.ObligacionesTributarias.Update(obligacion);
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<ObligacionTributaria> obligaciones)
    {
        dbContext.ObligacionesTributarias.UpdateRange(obligaciones);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}
