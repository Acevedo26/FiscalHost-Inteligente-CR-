using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.Audit;
using FiscalHost.Api.CR.Models.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Repositories;

public interface IAccesoContadorRepository
{
    Task<Usuario?> GetUsuarioAsync(Guid usuarioId);
    Task<Usuario?> GetUsuarioPorCorreoAsync(string correo);
    Task<AccesoContador?> GetByIdAsync(Guid accesoId);
    Task<AccesoContador?> GetActivoPorCorreoAsync(Guid anfitrionId, string correo);
    Task<List<AccesoContador>> GetByAnfitrionAsync(Guid anfitrionId);
    Task<List<AccesoContador>> GetExpiradosAsync(DateTimeOffset fechaReferencia);
    Task<List<AccesoContador>> GetPorVencerAsync(DateTimeOffset desde, DateTimeOffset hasta);
    Task AddAsync(AccesoContador acceso);
    Task AddAuditoriaAsync(AuditoriaOperacion auditoria);
    Task SaveChangesAsync();
}

public class AccesoContadorRepository(AppDbContext db) : IAccesoContadorRepository
{
    public Task<Usuario?> GetUsuarioAsync(Guid usuarioId) =>
        db.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);

    public Task<Usuario?> GetUsuarioPorCorreoAsync(string correo) =>
        db.Usuarios.FirstOrDefaultAsync(u =>
            u.CorreoElectronico.ToLower() == correo.ToLower());

    public Task<AccesoContador?> GetByIdAsync(Guid accesoId) =>
        db.AccesosContadores.FirstOrDefaultAsync(a => a.AccesoId == accesoId);

    public Task<AccesoContador?> GetActivoPorCorreoAsync(Guid anfitrionId, string correo) =>
        db.AccesosContadores.FirstOrDefaultAsync(a =>
            a.AnfitrionId == anfitrionId &&
            a.CorreoContador.ToLower() == correo.ToLower() &&
            a.Estado == "ACTIVO");

    public Task<List<AccesoContador>> GetByAnfitrionAsync(Guid anfitrionId) =>
        db.AccesosContadores
            .Where(a => a.AnfitrionId == anfitrionId)
            .OrderByDescending(a => a.FechaInvitacion)
            .ToListAsync();

    public Task<List<AccesoContador>> GetExpiradosAsync(DateTimeOffset fechaReferencia) =>
        db.AccesosContadores
            .Where(a => a.Estado == "ACTIVO" &&
                        a.FechaExpiracion.HasValue &&
                        a.FechaExpiracion <= fechaReferencia)
            .ToListAsync();

    public Task<List<AccesoContador>> GetPorVencerAsync(DateTimeOffset desde, DateTimeOffset hasta) =>
        db.AccesosContadores
            .Where(a => a.Estado == "ACTIVO" &&
                        a.FechaExpiracion.HasValue &&
                        a.FechaExpiracion > desde &&
                        a.FechaExpiracion <= hasta)
            .ToListAsync();

    public async Task AddAsync(AccesoContador acceso) =>
        await db.AccesosContadores.AddAsync(acceso);

    public async Task AddAuditoriaAsync(AuditoriaOperacion auditoria) =>
        await db.AuditoriasOperacion.AddAsync(auditoria);

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
