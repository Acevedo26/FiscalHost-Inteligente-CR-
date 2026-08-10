using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FiscalHost.Api.CR.Models.Entities.Communication;
using FiscalHost.Api.CR.Models.Entities.Identity;
using FiscalHost.Api.CR.Models.Enums.Identity;
using FiscalHost.Api.CR.Models.DTOs.Communication.Requests;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using NSubstitute;

namespace FiscalHost.Tests;

public class ContenidoEducativoServiceTests
{
	private readonly IContenidoEducativoRepository _repository = Substitute.For<IContenidoEducativoRepository>();
	private readonly IUsuarioRepository _usuarioRepository = Substitute.For<IUsuarioRepository>();
	private readonly INotificacionService _notificacionService = Substitute.For<INotificacionService>();
	private readonly ILogger<ContenidoEducativoService> _logger = Substitute.For<ILogger<ContenidoEducativoService>>();
	private readonly ContenidoEducativoService _sut;

	public ContenidoEducativoServiceTests()
	{
		_sut = new ContenidoEducativoService(_repository, _usuarioRepository, _notificacionService, _logger);
	}

	[Fact]
	public async Task Crear_DatosValidos_RetornaExitoConVersionInicial()
	{
		var request = BuildRequest();
		_repository.ExisteSlugAsync(request.Slug).Returns(false);

		var (success, error, data) = await _sut.CrearAsync(request, Guid.NewGuid());

		Assert.True(success);
		Assert.Null(error);
		Assert.Equal(1, data!.Version);
		Assert.Equal(request.Slug, data.Slug);
		await _repository.Received(1).AddAsync(Arg.Any<ContenidoEducativo>());
		await _repository.Received(1).SaveChangesAsync();
	}

	[Fact]
	public async Task Crear_SlugDuplicado_RetornaError()
	{
		var request = BuildRequest();
		_repository.ExisteSlugAsync(request.Slug).Returns(true);

		var (success, error, data) = await _sut.CrearAsync(request, Guid.NewGuid());

		Assert.False(success);
		Assert.Null(data);
		Assert.Contains("slug", error);
		await _repository.DidNotReceive().AddAsync(Arg.Any<ContenidoEducativo>());
	}

	[Fact]
	public async Task Crear_SlugConFormatoInvalido_RetornaError()
	{
		var request = BuildRequest();
		request.Slug = "IVA Deducible!!";

		var (success, error, _) = await _sut.CrearAsync(request, Guid.NewGuid());

		Assert.False(success);
		Assert.NotNull(error);
	}

	[Fact]
	public async Task Crear_TipoNoPermitido_RetornaError()
	{
		var request = BuildRequest();
		request.Tipo = "VIDEO";

		var (success, error, _) = await _sut.CrearAsync(request, Guid.NewGuid());

		Assert.False(success);
		Assert.NotNull(error);
	}

	[Fact]
	public async Task Crear_PublicadoTrue_AsignaFechaDePublicacion()
	{
		var request = BuildRequest();
		request.Publicado = true;
		_repository.ExisteSlugAsync(request.Slug).Returns(false);

		var (_, _, data) = await _sut.CrearAsync(request, Guid.NewGuid());

		Assert.True(data!.Publicado);
		Assert.NotNull(data.PublishedAt);
	}

	[Fact]
	public async Task ObtenerPorSlug_ContenidoExistente_RetornaDto()
	{
		var contenido = BuildContenido();
		_repository.GetBySlugAsync(contenido.Slug, true).Returns(contenido);

		var resultado = await _sut.ObtenerPorSlugAsync(contenido.Slug, incluirNoPublicados: false);

		Assert.NotNull(resultado);
		Assert.Equal(contenido.Titulo, resultado!.Titulo);
	}

	[Fact]
	public async Task ObtenerPorSlug_NoExiste_RetornaNull()
	{
		_repository.GetBySlugAsync(Arg.Any<string>(), Arg.Any<bool>()).Returns((ContenidoEducativo?)null);

		var resultado = await _sut.ObtenerPorSlugAsync("inexistente", incluirNoPublicados: false);

		Assert.Null(resultado);
	}

	[Fact]
	public async Task Actualizar_ContenidoExistente_IncrementaVersion()
	{
		var contenido = BuildContenido();
		contenido.Version = 1;
		_repository.GetByIdAsync(contenido.ContenidoId).Returns(contenido);

		var request = new ActualizarContenidoEducativoRequest
		{
			Titulo = "Título actualizado",
			ContenidoMarkdown = "Contenido actualizado.",
			Publicado = true,
			NotificarUsuarios = false,
		};

		var (success, error, data) = await _sut.ActualizarAsync(contenido.ContenidoId, request);

		Assert.True(success);
		Assert.Null(error);
		Assert.Equal(2, data!.Version);
		Assert.Equal("Título actualizado", data.Titulo);
		await _repository.Received(1).UpdateAsync(Arg.Any<ContenidoEducativo>());
		await _repository.Received(1).SaveChangesAsync();
	}

	[Fact]
	public async Task Actualizar_ContenidoInexistente_RetornaError()
	{
		_repository.GetByIdAsync(Arg.Any<Guid>()).Returns((ContenidoEducativo?)null);

		var (success, error, data) = await _sut.ActualizarAsync(Guid.NewGuid(), new ActualizarContenidoEducativoRequest
		{
			Titulo = "X",
			ContenidoMarkdown = "Y",
		});

		Assert.False(success);
		Assert.Null(data);
		Assert.NotNull(error);
	}

	[Fact]
	public async Task Actualizar_ConNotificarUsuariosYPublicado_NotificaSoloUsuariosActivos()
	{
		var contenido = BuildContenido();
		_repository.GetByIdAsync(contenido.ContenidoId).Returns(contenido);

		var usuarioActivo = BuildUsuario(EstadoUsuario.ACTIVO);
		var usuarioInactivo = BuildUsuario(EstadoUsuario.INACTIVO);
		_usuarioRepository.GetAllAsync().Returns(new List<Usuario> { usuarioActivo, usuarioInactivo });

		var request = new ActualizarContenidoEducativoRequest
		{
			Titulo = "Título actualizado",
			ContenidoMarkdown = "Contenido actualizado.",
			Publicado = true,
			NotificarUsuarios = true,
		};

		await _sut.ActualizarAsync(contenido.ContenidoId, request);

		await _notificacionService.Received(1).NotificarAsync(usuarioActivo.UsuarioId.ToString(), Arg.Any<string>());
		await _notificacionService.DidNotReceive().NotificarAsync(usuarioInactivo.UsuarioId.ToString(), Arg.Any<string>());
	}

	[Fact]
	public async Task Actualizar_ConNotificarUsuariosFalse_NoNotificaANadie()
	{
		var contenido = BuildContenido();
		_repository.GetByIdAsync(contenido.ContenidoId).Returns(contenido);

		var request = new ActualizarContenidoEducativoRequest
		{
			Titulo = "Título actualizado",
			ContenidoMarkdown = "Contenido actualizado.",
			Publicado = true,
			NotificarUsuarios = false,
		};

		await _sut.ActualizarAsync(contenido.ContenidoId, request);

		await _usuarioRepository.DidNotReceive().GetAllAsync();
		await _notificacionService.DidNotReceive().NotificarAsync(Arg.Any<string>(), Arg.Any<string>());
	}

	[Fact]
	public async Task ObtenerCategoriasDisponibles_RetornaListaDelRepositorio()
	{
		_repository.GetCategoriasDisponiblesAsync().Returns(new List<string> { "IVA", "Renta" });

		var categorias = await _sut.ObtenerCategoriasDisponiblesAsync();

		Assert.Equal(2, categorias.Count());
		Assert.Contains("IVA", categorias);
	}

	private static CreateContenidoEducativoRequest BuildRequest() => new()
	{
		Titulo = "¿Qué es el IVA deducible?",
		Slug = "iva-deducible",
		Categoria = "IVA",
		Tipo = "TOOLTIP",
		ContenidoMarkdown = "El IVA deducible es el monto que se resta del impuesto por gastos permitidos.",
		EsTutorialPrimerUso = false,
		OrdenDisplay = 1,
		Publicado = false,
	};

	private static ContenidoEducativo BuildContenido() => new()
	{
		ContenidoId = Guid.NewGuid(),
		Titulo = "¿Qué es el IVA deducible?",
		Slug = "iva-deducible",
		Categoria = "IVA",
		Tipo = "TOOLTIP",
		ContenidoMarkdown = "Contenido de prueba.",
		Version = 1,
		Publicado = true,
	};

	private static Usuario BuildUsuario(EstadoUsuario estado) => new()
	{
		UsuarioId = Guid.NewGuid(),
		TipoIdentificacion = TipoIdentificacion.FISICA,
		NumeroIdentificacion = Guid.NewGuid().ToString("N")[..9],
		NombreCompleto = "Usuario de Prueba",
		CorreoElectronico = $"{Guid.NewGuid()}@example.com",
		ContrasenaHash = "hash",
		Estado = estado,
		RolPrincipal = RolUsuario.ANFITRION,
		PreferenciasNotificacion = "{}",
	};
}