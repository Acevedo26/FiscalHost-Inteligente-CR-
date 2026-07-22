using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.Operations;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Repositories;

public interface IClasificacionIngresoRepository
{
    Task<ClasificacionIngreso?> GetByIdAsync(int id);
    Task AddAsync(ClasificacionIngreso clasificacion);
    Task AddAuditoriaAsync(AuditoriaClasificacionIngreso auditoria);
    Task SaveChangesAsync();
}

public class ClasificacionIngresoRepository(AppDbContext db) : IClasificacionIngresoRepository
{
    public Task<ClasificacionIngreso?> GetByIdAsync(int id) =>
        db.ClasificacionesIngresos
          .Include(c => c.Auditorias)
          .FirstOrDefaultAsync(c => c.Id == id);

    public async Task AddAsync(ClasificacionIngreso clasificacion) =>
        await db.ClasificacionesIngresos.AddAsync(clasificacion);

    public async Task AddAuditoriaAsync(AuditoriaClasificacionIngreso auditoria) =>
        await db.AuditoriasClasificacionIngresos.AddAsync(auditoria);

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
