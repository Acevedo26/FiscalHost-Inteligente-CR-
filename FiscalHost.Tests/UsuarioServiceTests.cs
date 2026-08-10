using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.Entities.Identity;
using FiscalHost.Api.CR.Models.DTOs.Identity.Requests;
using FiscalHost.Api.CR.Models.Enums.Communication;
using FiscalHost.Api.CR.Models.Enums.Identity;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using NSubstitute;
using Xunit;

namespace FiscalHost.Tests;

public class UsuarioServiceTests
{
	private readonly IUsuarioRepository _repository = Substitute.For<IUsuarioRepository>();
	private readonly UsuarioService _sut;

	public UsuarioServiceTests()
	{
		_sut = new UsuarioService(_repository);
	}

	[Fact]
	public async Task ObtenerTodos_RetornaTodosLosUsuariosMapeados()
	{
		var usuario = BuildUsuario();
		_repository.GetAllAsync().Returns(new List<Usuario> { usuario });

		var resultado = await _sut.ObtenerTodosAsync();

		var dto = Assert.Single(resultado);
		Assert.Equal(usuario.UsuarioId, dto.UsuarioId);
		Assert.Equal(usuario.NombreCompleto, dto.NombreCompleto);
	}

	[Fact]
	public async Task ObtenerPorId_UsuarioExistente_RetornaDto()
	{
		var usuario = BuildUsuario();
		_repository.GetByIdAsync(usuario.UsuarioId).Returns(usuario);

		var resultado = await _sut.ObtenerPorIdAsync(usuario.UsuarioId);

		Assert.NotNull(resultado);
		Assert.Equal(usuario.UsuarioId, resultado!.UsuarioId);
		Assert.Equal(usuario.EsUsuarioNuevo, resultado.EsUsuarioNuevo);
	}

	[Theory]
	[InlineData("{}", CanalNotificacion.AMBOS)] // Si el json de preferencias está vacío, AMBOS por defecto. 
	[InlineData("", CanalNotificacion.AMBOS)]
	[InlineData("json-invalido", CanalNotificacion.AMBOS)]
	[InlineData("{\"canalAlertas\":\"CORREO\"}", CanalNotificacion.CORREO)]
	[InlineData("{\"canalAlertas\":\"PLATAFORMA\"}", CanalNotificacion.PLATAFORMA)]
	public void ResolverCanalPreferido_DiferentesEntradas_RetornaCanalEsperado(string json, CanalNotificacion esperado)
	{
		var resultado = UsuarioService.ResolverCanalPreferido(json);
		Assert.Equal(esperado, resultado);
	}

	[Fact]
	public async Task ActualizarPreferencias_UsuarioExistente_GuardaCanalComoTexto()
	{
		var usuario = BuildUsuario();
		_repository.GetByIdAsync(usuario.UsuarioId).Returns(usuario);

		var (success, error, data) = await _sut.ActualizarPreferenciasNotificacionAsync(
			usuario.UsuarioId, new ActualizarPreferenciasNotificacionRequest { CanalAlertas = CanalNotificacion.CORREO });

		Assert.True(success);
		Assert.Null(error);
		Assert.Equal(CanalNotificacion.CORREO, data!.CanalAlertas);
		Assert.Contains("CORREO", usuario.PreferenciasNotificacion);
		await _repository.Received(1).SaveChangesAsync();
	}

	[Fact]
	public async Task ObtenerPorId_UsuarioInexistente_RetornaNull()
	{
		_repository.GetByIdAsync(Arg.Any<Guid>()).Returns((Usuario?)null);

		var resultado = await _sut.ObtenerPorIdAsync(Guid.NewGuid());

		Assert.Null(resultado);
	}

	[Fact]
	public async Task CompletarTutorial_UsuarioNuevo_MarcaComoNoNuevo()
	{
		var usuario = BuildUsuario();
		usuario.EsUsuarioNuevo = true;
		_repository.GetByIdAsync(usuario.UsuarioId).Returns(usuario);

		var (success, error) = await _sut.MarcarTutorialCompletadoAsync(usuario.UsuarioId);

		Assert.True(success);
		Assert.Null(error);
		Assert.False(usuario.EsUsuarioNuevo);
		await _repository.Received(1).SaveChangesAsync();
	}

	[Fact]
	public async Task CompletarTutorial_UsuarioYaNoEsNuevo_NoGuardaCambiosInnecesarios()
	{
		var usuario = BuildUsuario();
		usuario.EsUsuarioNuevo = false;
		_repository.GetByIdAsync(usuario.UsuarioId).Returns(usuario);

		var (success, _) = await _sut.MarcarTutorialCompletadoAsync(usuario.UsuarioId);

		Assert.True(success);
		await _repository.DidNotReceive().SaveChangesAsync();
	}

	[Fact]
	public async Task CompletarTutorial_UsuarioInexistente_RetornaError()
	{
		_repository.GetByIdAsync(Arg.Any<Guid>()).Returns((Usuario?)null);

		var (success, error) = await _sut.MarcarTutorialCompletadoAsync(Guid.NewGuid());

		Assert.False(success);
		Assert.NotNull(error);
	}
    
	[Fact]
	public async Task ActualizarPreferencias_UsuarioInexistente_RetornaError()
	{
		_repository.GetByIdAsync(Arg.Any<Guid>()).Returns((Usuario?)null);

		var (success, error, data) = await _sut.ActualizarPreferenciasNotificacionAsync(
			Guid.NewGuid(), new ActualizarPreferenciasNotificacionRequest { CanalAlertas = CanalNotificacion.CORREO });

		Assert.False(success);
		Assert.Null(data);
		Assert.NotNull(error);
	}

	[Fact]
	public async Task ObtenerPreferencias_UsuarioExistente_RetornaCanalActual()
	{
		var usuario = BuildUsuario();
		usuario.PreferenciasNotificacion = "{\"canalAlertas\":\"PLATAFORMA\"}";
		_repository.GetByIdAsync(usuario.UsuarioId).Returns(usuario);

		var resultado = await _sut.ObtenerPreferenciasNotificacionAsync(usuario.UsuarioId);

		Assert.NotNull(resultado);
		Assert.Equal(CanalNotificacion.PLATAFORMA, resultado!.CanalAlertas);
	}

	private static Usuario BuildUsuario() => new()
	{
		UsuarioId = Guid.NewGuid(),
		TipoIdentificacion = TipoIdentificacion.FISICA,
		NumeroIdentificacion = "1-1111-1111",
		NombreCompleto = "Usuario de Prueba",
		CorreoElectronico = "prueba@example.com",
		ContrasenaHash = "hash",
		Estado = EstadoUsuario.ACTIVO,
		RolPrincipal = RolUsuario.ANFITRION,
		EsUsuarioNuevo = true,
		PreferenciasNotificacion = "{}",
	};
}