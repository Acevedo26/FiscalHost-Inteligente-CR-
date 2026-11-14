using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums.TaxIntelligence;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Repositories;

public class PeriodoFiscalRepository(AppDbContext db) : IPeriodoFiscalRepository
{
    public async Task<PeriodoFiscal?> GetByAnioMesTipoAsync(short anio, short mes, TipoFormulario tipoFormulario) =>
        await db.PeriodosFiscales
            .FirstOrDefaultAsync(p => p.Anio == anio && p.Mes == mes && p.TipoFormulario == tipoFormulario);

    public async Task<List<PeriodoFiscal>> GetByAnioRangoTipoAsync(short anioInicio, short anioFin, TipoFormulario tipoFormulario) =>
        await db.PeriodosFiscales
            .Where(p => p.Anio >= anioInicio && p.Anio <= anioFin && p.TipoFormulario == tipoFormulario)
            .OrderBy(p => p.Anio)
            .ThenBy(p => p.Mes)
            .ToListAsync();

    public async Task<bool> ExisteAsync(short anio, short mes, TipoFormulario tipoFormulario) =>
        await db.PeriodosFiscales
            .AnyAsync(p => p.Anio == anio && p.Mes == mes && p.TipoFormulario == tipoFormulario);
}