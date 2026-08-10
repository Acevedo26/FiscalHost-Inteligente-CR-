using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> ObtenerTodosAsync();
	Task<UsuarioDto?> ObtenerPorIdAsync(Guid usuarioId);
	Task<(bool success, string? error)> MarcarTutorialCompletadoAsync(Guid usuarioId);
}

public class UsuarioService(IUsuarioRepository usuarioRepo) : IUsuarioService
{
    public async Task<IEnumerable<UsuarioDto>> ObtenerTodosAsync()
    {
        var usuarios = await usuarioRepo.GetAllAsync();
        
        return usuarios.Select(u => new UsuarioDto
        {
            UsuarioId = u.UsuarioId,
            TipoIdentificacion = u.TipoIdentificacion,
            NumeroIdentificacion = u.NumeroIdentificacion,
            NombreCompleto = u.NombreCompleto,
            RazonSocial = u.RazonSocial,
            CorreoElectronico = u.CorreoElectronico,
            Estado = u.Estado,
            RolPrincipal = u.RolPrincipal,
            EsUsuarioNuevo = u.EsUsuarioNuevo,
            CorreoVerificado = u.CorreoVerificado,
            FechaActivacion = u.FechaActivacion,
            UltimoAcceso = u.UltimoAcceso
        });
    }

	// RF-019 - Escenario "Tutorial de primer uso": "El usuario puede omitir el tutorial".
	// Marca al usuario como no-nuevo, para que no se le vuelva a mostrar el recorrido guiado.
	public async Task<(bool success, string? error)> MarcarTutorialCompletadoAsync(Guid usuarioId)
	{
		var usuario = await usuarioRepo.GetByIdAsync(usuarioId);
		if (usuario == null)
		{
			return (false, "Usuario no encontrado.");
		}

		if (usuario.EsUsuarioNuevo)
		{
			usuario.EsUsuarioNuevo = false;
			await usuarioRepo.UpdateAsync(usuario);
			await usuarioRepo.SaveChangesAsync();
		}

		return (true, null);
	}

	// RF-019 - Escenario "Tutorial de primer uso": "El sistema detecta usuarios nuevos".
	// Permite consultar un solo usuario (en vez de tener que traer la lista completa)
	// para que el frontend decida si debe mostrarle el recorrido guiado.
	public async Task<UsuarioDto?> ObtenerPorIdAsync(Guid usuarioId)
	{
		var usuario = await usuarioRepo.GetByIdAsync(usuarioId);
		if (usuario == null)
		{
			return null;
		}

		return new UsuarioDto
		{
			UsuarioId = usuario.UsuarioId,
			TipoIdentificacion = usuario.TipoIdentificacion,
			NumeroIdentificacion = usuario.NumeroIdentificacion,
			NombreCompleto = usuario.NombreCompleto,
			RazonSocial = usuario.RazonSocial,
			CorreoElectronico = usuario.CorreoElectronico,
			Estado = usuario.Estado,
			RolPrincipal = usuario.RolPrincipal,
			EsUsuarioNuevo = usuario.EsUsuarioNuevo,
			CorreoVerificado = usuario.CorreoVerificado,
			FechaActivacion = usuario.FechaActivacion,
			UltimoAcceso = usuario.UltimoAcceso
		};
	}
}
