using FiscalHost.Api.CR.Data;

using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByCorreoAsync(string correo);
    Task<Usuario?> GetByIdentificacionAsync(string identificacion);
    Task<IEnumerable<Usuario>> GetAllAsync();
    Task AddAsync(Usuario usuario);
    Task SaveChangesAsync();
}

public class UsuarioRepository(AppDbContext db) : IUsuarioRepository
{
    public Task<Usuario?> GetByCorreoAsync(string correo) =>
        db.Usuarios.FirstOrDefaultAsync(u =>
            u.CorreoElectronico.ToLower() == correo.ToLower());

    public Task<Usuario?> GetByIdentificacionAsync(string identificacion) =>
        db.Usuarios.FirstOrDefaultAsync(u =>
            u.NumeroIdentificacion == identificacion);

    public async Task<IEnumerable<Usuario>> GetAllAsync() =>
        await db.Usuarios.ToListAsync();

    public async Task AddAsync(Usuario usuario)
    {
        usuario.UsuarioId = Guid.NewGuid();
        await db.Usuarios.AddAsync(usuario);
    }

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
