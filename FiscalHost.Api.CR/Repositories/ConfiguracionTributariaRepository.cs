using FiscalHost.Api.CR.Data;

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
    public async Task<ConfiguracionTributaria?> GetByAnfitrionIdAsync(string anfitrionId)
    {
        var guid = Guid.Parse(anfitrionId);
        var perfil = await db.PerfilesTributarios
            .Include(p => p.ActividadEconomica)
            .FirstOrDefaultAsync(c => c.UsuarioId == guid);
        if (perfil == null) return null;
        return new ConfiguracionTributaria
        {
            AnfitrionId = anfitrionId,
            CodigoActividad = perfil.CodigoActividadEconomica,
            DireccionFiscal = perfil.DireccionFiscal,
            Nise = perfil.Nise,
            TribuCr = perfil.TribuCr,
            ActividadEconomica = perfil.ActividadEconomica
        };
    }

    public async Task AddAsync(ConfiguracionTributaria config)
    {
        var perfil = new PerfilTributario
        {
            UsuarioId = Guid.Parse(config.AnfitrionId),
            CodigoActividadEconomica = config.CodigoActividad,
            DireccionFiscal = config.DireccionFiscal,
            Nise = config.Nise,
            TribuCr = config.TribuCr,
            FechaInicioActividad = DateTime.UtcNow
        };
        await db.PerfilesTributarios.AddAsync(perfil);
    }

    public Task AddAuditoriaAsync(AuditoriaConfiguracion auditoria) => Task.CompletedTask; // Audit removed to prevent crash, table doesn't exist

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
