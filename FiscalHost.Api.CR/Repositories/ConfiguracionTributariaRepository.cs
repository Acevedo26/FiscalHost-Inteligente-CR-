using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Repositories;

public interface IConfiguracionTributariaRepository
{
    Task<ConfiguracionTributaria?> GetByAnfitrionIdAsync(string anfitrionId);
    Task AddAsync(ConfiguracionTributaria config);
    Task AddAuditoriaAsync(AuditoriaConfiguracion auditoria);
    Task SaveChangesAsync();
}

public class ConfiguracionTributariaRepository(AppDbContext db) : IConfiguracionTributariaRepository
{
    public Task<ConfiguracionTributaria?> GetByAnfitrionIdAsync(string anfitrionId) =>
        db.ConfiguracionesTributarias
          .Include(c => c.ActividadEconomica)
          .FirstOrDefaultAsync(c => c.AnfitrionId == anfitrionId);

    public async Task AddAsync(ConfiguracionTributaria config) => await db.ConfiguracionesTributarias.AddAsync(config);

    public async Task AddAuditoriaAsync(AuditoriaConfiguracion auditoria) => await db.AuditoriasConfiguracion.AddAsync(auditoria);

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
