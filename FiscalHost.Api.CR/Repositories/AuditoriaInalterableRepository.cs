using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.Audit;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Repositories;

public interface IAuditoriaInalterableRepository
{
    Task AddAsync(AuditoriaOperacion auditoria);
    Task<List<AuditoriaOperacion>> GetHistorialAsync(Guid? usuarioId, string? tablaAfectada, Guid? registroId);
    Task SaveChangesAsync();
}

public class AuditoriaInalterableRepository(AppDbContext db)
    : IAuditoriaInalterableRepository
{
    public async Task AddAsync(AuditoriaOperacion auditoria) =>
        await db.AuditoriasOperacion.AddAsync(auditoria);

    public Task<List<AuditoriaOperacion>> GetHistorialAsync(
        Guid? usuarioId,
        string? tablaAfectada,
        Guid? registroId)
    {
        var query = db.AuditoriasOperacion.AsQueryable();

        if (usuarioId.HasValue)
            query = query.Where(a => a.UsuarioId == usuarioId.Value);

        if (!string.IsNullOrWhiteSpace(tablaAfectada))
            query = query.Where(a => a.TablaAfectada == tablaAfectada);

        if (registroId.HasValue)
            query = query.Where(a => a.RegistroId == registroId.Value);

        return query
            .OrderBy(a => a.CreatedAt)
            .ToListAsync();
    }

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
