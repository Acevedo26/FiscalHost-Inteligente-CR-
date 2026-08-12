using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.Identity;
using FiscalHost.Api.CR.Models.Enums.Identity;
using FiscalHost.Api.CR.Models.DTOs.Communication.Requests;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;

namespace FiscalHost.Tests.Integration;

public class ContenidoEducativoIntegrationTests : IDisposable
{
	private readonly AppDbContext _db;
	private readonly INotificacionService _notificacionService = Substitute.For<INotificacionService>();
	private readonly ContenidoEducativoService _sut;

	public ContenidoEducativoIntegrationTests()
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;

		_db = new AppDbContext(options);

		var contenidoRepository = new ContenidoEducativoRepository(_db);
		var usuarioRepository = new UsuarioRepository(_db);
		var logger = Substitute.For<ILogger<ContenidoEducativoService>>();

		_sut = new ContenidoEducativoService(contenidoRepository, usuarioRepository, _notificacionService, logger);
	}

	[Fact]
	public async Task CrearYConsultar_FlujoCompleto_PersisteYRecuperaPorSlug()
	{
		var request = BuildRequest();
		request.Publicado = true;

		var (success, error, data) = await _sut.CrearAsync(request, autorId: null);

		Assert.True(success);
		Assert.Null(error);

		var recuperado = await _sut.ObtenerPorSlugAsync(request.Slug, incluirNoPublicados: false);

		Assert.NotNull(recuperado);
		Assert.Equal(data!.ContenidoId, recuperado!.ContenidoId);
	}

	[Fact]
	public async Task Actualizar_ConUsuariosActivosEnBd_NotificaAlUsuarioActivo()
	{
		var usuario = new Usuario
		{
			UsuarioId = Guid.NewGuid(),
			TipoIdentificacion = TipoIdentificacion.FISICA,
			NumeroIdentificacion = "1-2222-2222",
			NombreCompleto = "Usuario Activo",
			CorreoElectronico = "activo@example.com",
			ContrasenaHash = "hash",
			Estado = EstadoUsuario.ACTIVO,
			RolPrincipal = RolUsuario.ANFITRION,
			PreferenciasNotificacion = "{}",
		};
		_db.Usuarios.Add(usuario);
		await _db.SaveChangesAsync();

		var (_, _, creado) = await _sut.CrearAsync(BuildRequest(), autorId: null);

		var request = new ActualizarContenidoEducativoRequest
		{
			Titulo = "Título actualizado",
			ContenidoMarkdown = "Contenido actualizado con más detalle.",
			Publicado = true,
			NotificarUsuarios = true,
		};

		await _sut.ActualizarAsync(creado!.ContenidoId, request);

		await _notificacionService.Received(1).NotificarAsync(usuario.UsuarioId.ToString(), Arg.Any<string>());
	}

	[Fact]
	public async Task ObtenerCategoriasDisponibles_ContenidoPublicadoEnBd_RetornaCategoria()
	{
		var request = BuildRequest();
		request.Publicado = true;
		await _sut.CrearAsync(request, autorId: null);

		var categorias = await _sut.ObtenerCategoriasDisponiblesAsync();

		Assert.Contains("IVA", categorias);
	}

	private static CreateContenidoEducativoRequest BuildRequest() => new()
	{
		Titulo = "¿Qué es el IVA deducible?",
		Slug = $"iva-deducible-{Guid.NewGuid():N}"[..20],
		Categoria = "IVA",
		Tipo = "TOOLTIP",
		ContenidoMarkdown = "El IVA deducible es el monto que se resta del impuesto por gastos permitidos.",
		EsTutorialPrimerUso = false,
		OrdenDisplay = 1,
		Publicado = false,
	};

	public void Dispose() => _db.Dispose();
}