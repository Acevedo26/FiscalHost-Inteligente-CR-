using System.Text.Json;
using System.Text.Json.Serialization;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> ObtenerTodosAsync();
	Task<PreferenciasNotificacionDto?> ObtenerPreferenciasNotificacionAsync(Guid usuarioId);
	Task<(bool success, string? error, PreferenciasNotificacionDto? data)> ActualizarPreferenciasNotificacionAsync(
		Guid usuarioId, ActualizarPreferenciasNotificacionRequest request);
}

public class UsuarioService(IUsuarioRepository usuarioRepo) : IUsuarioService
{
	private static readonly JsonSerializerOptions PreferenciasJsonOptions = new()
	{
		Converters = { new JsonStringEnumConverter() },
		PropertyNameCaseInsensitive = true,
	};

	private class PreferenciasNotificacionJson
	{
		public CanalNotificacion? CanalAlertas { get; set; }
	}
	
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

	public async Task<PreferenciasNotificacionDto?> ObtenerPreferenciasNotificacionAsync(Guid usuarioId)
	{
		var usuario = await usuarioRepo.GetByIdAsync(usuarioId);
		if (usuario == null)
		{
			return null;
		}

		return new PreferenciasNotificacionDto
		{
			CanalAlertas = ResolverCanalPreferido(usuario.PreferenciasNotificacion)
		};
	}

	public async Task<(bool success, string? error, PreferenciasNotificacionDto? data)> ActualizarPreferenciasNotificacionAsync(
		Guid usuarioId, ActualizarPreferenciasNotificacionRequest request)
	{
		var usuario = await usuarioRepo.GetByIdAsync(usuarioId);
		if (usuario == null)
		{
			return (false, "Usuario no encontrado.", null);
		}

		var preferencias = new PreferenciasNotificacionJson { CanalAlertas = request.CanalAlertas };
		usuario.PreferenciasNotificacion = JsonSerializer.Serialize(preferencias, PreferenciasJsonOptions);

		await usuarioRepo.UpdateAsync(usuario);
		await usuarioRepo.SaveChangesAsync();

		return (true, null, new PreferenciasNotificacionDto { CanalAlertas = request.CanalAlertas });
	}

	// RF-013 - "El sistema respeta los canales seleccionados": si el usuario no ha
	// configurado preferencia (JSON vacío o inválido), se usa AMBOS como valor por defecto,
	// preservando el comportamiento previo a esta funcionalidad.
	public static CanalNotificacion ResolverCanalPreferido(string preferenciasNotificacionJson)
	{
		if (string.IsNullOrWhiteSpace(preferenciasNotificacionJson))
		{
			return CanalNotificacion.AMBOS;
		}

		try
		{
			var preferencias = JsonSerializer.Deserialize<PreferenciasNotificacionJson>(preferenciasNotificacionJson, PreferenciasJsonOptions);
			return preferencias?.CanalAlertas ?? CanalNotificacion.AMBOS;
		}
		catch (JsonException)
		{
			return CanalNotificacion.AMBOS;
		}
	}
}


