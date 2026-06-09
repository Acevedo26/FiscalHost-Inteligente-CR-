using FiscalHost.Api.CR.Models.Entities;

namespace FiscalHost.Api.CR.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByCorreoAsync(string correo);

    Task AddAsync(Usuario usuario);

    Task SaveChangesAsync();
}

public class UsuarioRepository : IUsuarioRepository
{
    private readonly List<Usuario> _usuarios = [];

    public Task<Usuario?> GetByCorreoAsync(string correo)
    {
        return Task.FromResult(
            _usuarios.FirstOrDefault(u =>
                u.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase))
        );
    }

    public Task AddAsync(Usuario usuario)
    {
        usuario.Id = _usuarios.Count + 1;
        _usuarios.Add(usuario);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}