using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.DTOs;
using FiscalHost.Api.CR.Models.DTOs.Identity.Requests;
using FiscalHost.Api.CR.Models.DTOs.Identity.Responses;
using FiscalHost.Api.CR.Models.Enums.Communication;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public interface IUsuarioService
{
	Task<IEnumerable<UsuarioDto>> ObtenerTodosAsync();
	Task<UsuarioDto?> ObtenerPorIdAsync(Guid usuarioId);
	Task<(bool success, string? error)> MarcarTutorialCompletadoAsync(Guid usuarioId);
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

	public static CanalNotificacion ResolverCanalPreferido(string jsonPrefs)
	{
		if (string.IsNullOrWhiteSpace(jsonPrefs))
			return CanalNotificacion.AMBOS;

		try
		{
			var prefs = JsonSerializer.Deserialize<PreferenciasNotificacionJson>(jsonPrefs, PreferenciasJsonOptions);
			return prefs?.CanalAlertas ?? CanalNotificacion.AMBOS;
		}
		catch
		{
			return CanalNotificacion.AMBOS;
		}
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

	public async Task<PreferenciasNotificacionDto?> ObtenerPreferenciasNotificacionAsync(Guid usuarioId)
	{
		var usuario = await usuarioRepo.GetByIdAsync(usuarioId);
		if (usuario == null) return null;

		var prefs = string.IsNullOrEmpty(usuario.PreferenciasNotificacion)
			? new PreferenciasNotificacionJson()
			: JsonSerializer.Deserialize<PreferenciasNotificacionJson>(usuario.PreferenciasNotificacion, PreferenciasJsonOptions) 
				?? new PreferenciasNotificacionJson();

		return new PreferenciasNotificacionDto
		{
			CanalAlertas = prefs.CanalAlertas ?? CanalNotificacion.AMBOS
		};
	}

	public async Task<(bool success, string? error, PreferenciasNotificacionDto? data)> ActualizarPreferenciasNotificacionAsync(
		Guid usuarioId, ActualizarPreferenciasNotificacionRequest request)
	{
		var usuario = await usuarioRepo.GetByIdAsync(usuarioId);
		if (usuario == null) return (false, "Usuario no encontrado.", null);

		var prefs = string.IsNullOrEmpty(usuario.PreferenciasNotificacion)
			? new PreferenciasNotificacionJson()
			: JsonSerializer.Deserialize<PreferenciasNotificacionJson>(usuario.PreferenciasNotificacion, PreferenciasJsonOptions) 
				?? new PreferenciasNotificacionJson();

		prefs.CanalAlertas = request.CanalAlertas;
		usuario.PreferenciasNotificacion = JsonSerializer.Serialize(prefs, PreferenciasJsonOptions);

		await usuarioRepo.UpdateAsync(usuario);
		await usuarioRepo.SaveChangesAsync();

		return (true, null, new PreferenciasNotificacionDto { CanalAlertas = prefs.CanalAlertas ?? CanalNotificacion.AMBOS });
	}
}
