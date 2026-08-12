using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FiscalHost.Api.CR.Models.Entities.Communication;
using FiscalHost.Api.CR.Models.Enums.Identity;
using FiscalHost.Api.CR.Models.DTOs.Communication.Requests;
using FiscalHost.Api.CR.Models.DTOs.Communication.Responses;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public partial class ContenidoEducativoService(
	IContenidoEducativoRepository repository,
	IUsuarioRepository usuarioRepository,
	INotificacionService notificacionService,
	ILogger<ContenidoEducativoService> logger) : IContenidoEducativoService
{
	private static readonly string[] TiposPermitidos = ["GUIA", "TOOLTIP", "TUTORIAL", "FAQ"];

	public async Task<ContenidoEducativoDto?> ObtenerPorSlugAsync(string slug, bool incluirNoPublicados)
	{
		var contenido = await repository.GetBySlugAsync(slug, soloPublicados: !incluirNoPublicados);
		return contenido == null ? null : MapToDto(contenido);
	}

	public async Task<IEnumerable<ContenidoEducativoDto>> ListarPorCategoriaAsync(string categoria, bool incluirNoPublicados)
	{
		var contenidos = await repository.GetByCategoriaAsync(categoria, soloPublicados: !incluirNoPublicados);
		return contenidos.Select(MapToDto);
	}

	public async Task<IEnumerable<ContenidoEducativoDto>> ListarTutorialesPrimerUsoAsync()
	{
		var contenidos = await repository.GetTutorialesPrimerUsoAsync(soloPublicados: true);
		return contenidos.Select(MapToDto);
	}

	public async Task<(bool success, string? error, ContenidoEducativoDto? data)> CrearAsync(
		CreateContenidoEducativoRequest request, Guid? autorId)
	{
		var error = await ValidarAsync(request);
		if (error is not null)
		{
			return (false, error, null);
		}

		var ahora = DateTimeOffset.UtcNow;
		var contenido = new ContenidoEducativo
		{
			ContenidoId = Guid.NewGuid(),
			Titulo = request.Titulo.Trim(),
			Slug = request.Slug.Trim().ToLowerInvariant(),
			Categoria = request.Categoria.Trim(),
			Tipo = request.Tipo.Trim().ToUpperInvariant(),
			ContenidoMarkdown = request.ContenidoMarkdown,
			ContenidoHtml = RenderizarHtml(request.ContenidoMarkdown),
			EsTutorialPrimerUso = request.EsTutorialPrimerUso,
			OrdenDisplay = request.OrdenDisplay,
			Version = 1,
			Publicado = request.Publicado,
			AutorId = autorId,
			PublishedAt = request.Publicado ? ahora : null,
		};

		await repository.AddAsync(contenido);
		await repository.SaveChangesAsync();

		return (true, null, MapToDto(contenido));
	}

	// RF-019 - Escenario "Actualización de contenido": los administradores pueden
	// publicar cambios sobre una guía existente, y el sistema notifica a los
	// usuarios activos que el contenido disponible cambió.
	public async Task<(bool success, string? error, ContenidoEducativoDto? data)> ActualizarAsync(
		Guid contenidoId, ActualizarContenidoEducativoRequest request)
	{
		var contenido = await repository.GetByIdAsync(contenidoId);
		if (contenido == null)
		{
			return (false, "Contenido educativo no encontrado.", null);
		}

		if (string.IsNullOrWhiteSpace(request.Titulo))
		{
			return (false, "El título es obligatorio.", null);
		}

		if (string.IsNullOrWhiteSpace(request.ContenidoMarkdown))
		{
			return (false, "El contenido no puede estar vacío.", null);
		}

		var seEstaPublicandoPorPrimeraVez = request.Publicado && !contenido.Publicado;

		contenido.Titulo = request.Titulo.Trim();
		contenido.ContenidoMarkdown = request.ContenidoMarkdown;
		contenido.ContenidoHtml = RenderizarHtml(request.ContenidoMarkdown);
		contenido.Publicado = request.Publicado;
		contenido.Version++;

		if (seEstaPublicandoPorPrimeraVez)
		{
			contenido.PublishedAt = DateTimeOffset.UtcNow;
		}

		await repository.UpdateAsync(contenido);
		await repository.SaveChangesAsync();

		if (request.NotificarUsuarios && contenido.Publicado)
		{
			await NotificarActualizacionAsync(contenido);
		}

		return (true, null, MapToDto(contenido));
	}

	// RF-019 - Escenario "Contenido inexistente": si el slug/categoría solicitados
	// no existen, se ofrecen las categorías disponibles como alternativa de consulta.
	public async Task<IEnumerable<string>> ObtenerCategoriasDisponiblesAsync()
	{
		return await repository.GetCategoriasDisponiblesAsync();
	}

	private async Task NotificarActualizacionAsync(ContenidoEducativo contenido)
	{
		var usuarios = await usuarioRepository.GetAllAsync();
		var usuariosActivos = usuarios.Where(u => u.Estado == EstadoUsuario.ACTIVO).ToList();

		if (usuariosActivos.Count == 0)
		{
			return;
		}

		var mensaje = $"La guía '{contenido.Titulo}' fue actualizada. Consúltala para ver los cambios más recientes.";

		foreach (var usuario in usuariosActivos)
		{
			await notificacionService.NotificarAsync(usuario.UsuarioId.ToString(), mensaje);
		}

		logger.LogInformation("Se notificó la actualización del contenido '{Slug}' a {Cantidad} usuarios.", contenido.Slug, usuariosActivos.Count);
	}

	private async Task<string?> ValidarAsync(CreateContenidoEducativoRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.Titulo))
			return "El título es obligatorio.";

		if (string.IsNullOrWhiteSpace(request.Slug) || !SlugRegex().IsMatch(request.Slug))
			return "El slug debe contener solo minúsculas, números y guiones (ej: 'iva-deducible').";

		if (string.IsNullOrWhiteSpace(request.Categoria))
			return "La categoría es obligatoria.";

		if (string.IsNullOrWhiteSpace(request.Tipo) || !TiposPermitidos.Contains(request.Tipo.Trim().ToUpperInvariant()))
			return $"El tipo debe ser uno de: {string.Join(", ", TiposPermitidos)}.";

		if (string.IsNullOrWhiteSpace(request.ContenidoMarkdown))
			return "El contenido no puede estar vacío.";

		if (await repository.ExisteSlugAsync(request.Slug.Trim().ToLowerInvariant()))
			return $"Ya existe contenido educativo con el slug '{request.Slug}'.";

		return null;
	}

	private static string RenderizarHtml(string markdown)
	{
		var parrafos = markdown
			.Split(["\r\n\r\n", "\n\n"], StringSplitOptions.RemoveEmptyEntries)
			.Select(p => $"<p>{p.Trim()}</p>");

		return string.Join(Environment.NewLine, parrafos);
	}

	private static ContenidoEducativoDto MapToDto(ContenidoEducativo contenido) => new()
	{
		ContenidoId = contenido.ContenidoId,
		Titulo = contenido.Titulo,
		Slug = contenido.Slug,
		Categoria = contenido.Categoria,
		Tipo = contenido.Tipo,
		ContenidoMarkdown = contenido.ContenidoMarkdown,
		ContenidoHtml = contenido.ContenidoHtml,
		EsTutorialPrimerUso = contenido.EsTutorialPrimerUso,
		OrdenDisplay = contenido.OrdenDisplay,
		Version = contenido.Version,
		Publicado = contenido.Publicado,
		AutorId = contenido.AutorId,
		PublishedAt = contenido.PublishedAt,
	};

	[GeneratedRegex("^[a-z0-9]+(-[a-z0-9]+)*$")]
	private static partial Regex SlugRegex();
}