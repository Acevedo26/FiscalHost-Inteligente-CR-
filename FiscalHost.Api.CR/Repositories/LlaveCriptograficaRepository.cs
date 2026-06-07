using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Repositories;

public interface ILlaveCriptograficaRepository
{
    Task<LlaveCriptografica?> GetByAnfitrionIdAsync(string anfitrionId);
    Task AddAsync(LlaveCriptografica llave);
    Task AddAuditoriaAsync(AuditoriaLlave auditoria);
    Task SaveChangesAsync();
}

public class LlaveCriptograficaRepository(AppDbContext db) : ILlaveCriptograficaRepository
{
    public Task<LlaveCriptografica?> GetByAnfitrionIdAsync(string anfitrionId) =>
        db.LlavesCriptograficas.FirstOrDefaultAsync(l => l.AnfitrionId == anfitrionId && l.Activa);

    public async Task AddAsync(LlaveCriptografica llave) => await db.LlavesCriptograficas.AddAsync(llave);

    public async Task AddAuditoriaAsync(AuditoriaLlave auditoria) => await db.AuditoriasLlave.AddAsync(auditoria);

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
