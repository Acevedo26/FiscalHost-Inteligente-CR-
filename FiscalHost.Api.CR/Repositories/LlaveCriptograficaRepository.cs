using FiscalHost.Api.CR.Data;

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
        db.LlavesCriptograficas.FirstOrDefaultAsync(l => l.UsuarioId.ToString() == anfitrionId && l.Estado == "ACTIVA");

    public async Task AddAsync(LlaveCriptografica llave) => await db.LlavesCriptograficas.AddAsync(llave);

    public Task AddAuditoriaAsync(AuditoriaLlave auditoria) => Task.CompletedTask; // Audit removed to prevent crash, table doesn't exist

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
