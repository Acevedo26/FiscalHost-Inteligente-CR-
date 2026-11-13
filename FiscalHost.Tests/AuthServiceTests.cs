using FiscalHost.Api.CR.Models.DTOs;
using FiscalHost.Api.CR.Models.Entities;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using NSubstitute;
using Microsoft.Extensions.Configuration;

namespace FiscalHost.Tests;

public class AuthServiceTests
{
    private readonly IUsuarioRepository _usuarioRepo =
        Substitute.For<IUsuarioRepository>();
        
    private readonly IConfiguration _config = 
        Substitute.For<IConfiguration>();

    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(_usuarioRepo, _config);
    }

    // Escenario: Registro exitoso con cédula física
    [Fact]
    public async Task RegistrarUsuario_CedulaFisicaValida_CreaUsuario()
    {
        _usuarioRepo.GetByCorreoAsync("enzo@test.com")
            .Returns((Usuario?)null);

        var request = BuildRequest(
            correo: "enzo@test.com",
            tipoIdentificacion: "Fisica",
            identificacion: "123456789");

        var (success, error, data) =
            await _sut.RegistrarUsuarioAsync(request);

        Assert.True(success);
        Assert.Null(error);
        Assert.NotNull(data);

        await _usuarioRepo.Received(1)
            .AddAsync(Arg.Any<Usuario>());
    }

    // Escenario: Registro exitoso con DIMEX
    [Fact]
    public async Task RegistrarUsuario_DIMEXValido_CreaUsuario()
    {
        _usuarioRepo.GetByCorreoAsync("dimex@test.com")
            .Returns((Usuario?)null);

        var request = BuildRequest(
            correo: "dimex@test.com",
            tipoIdentificacion: "DIMEX",
            identificacion: "12345678901");

        var (success, error, data) =
            await _sut.RegistrarUsuarioAsync(request);

        Assert.True(success);
        Assert.Null(error);
        Assert.NotNull(data);
    }

    // Escenario: Registro exitoso con NITE
    [Fact]
    public async Task RegistrarUsuario_NITEValido_CreaUsuario()
    {
        _usuarioRepo.GetByCorreoAsync("nite@test.com")
            .Returns((Usuario?)null);

        var request = BuildRequest(
            correo: "nite@test.com",
            tipoIdentificacion: "NITE",
            identificacion: "1234567890");

        var (success, error, data) =
            await _sut.RegistrarUsuarioAsync(request);

        Assert.True(success);
        Assert.Null(error);
        Assert.NotNull(data);
    }

    // Escenario: Correo ya registrado
    [Fact]
    public async Task RegistrarUsuario_CorreoDuplicado_RetornaError()
    {
        _usuarioRepo.GetByCorreoAsync("duplicado@test.com")
            .Returns(new Usuario
            {
                UsuarioId = Guid.NewGuid(),
                CorreoElectronico = "duplicado@test.com"
            });

        var request = BuildRequest(
            correo: "duplicado@test.com");

        var (success, error, data) =
            await _sut.RegistrarUsuarioAsync(request);

        Assert.False(success);
        Assert.Contains("correo", error!,
            StringComparison.OrdinalIgnoreCase);
        Assert.Null(data);
    }

    // Escenario: Contraseña inválida
    [Fact]
    public async Task RegistrarUsuario_ContrasenaInvalida_RetornaError()
    {
        _usuarioRepo.GetByCorreoAsync("password@test.com")
            .Returns((Usuario?)null);

        var request = BuildRequest(
            password: "abc123");

        var (success, error, data) =
            await _sut.RegistrarUsuarioAsync(request);

        Assert.False(success);
        Assert.Contains("contraseña", error!,
            StringComparison.OrdinalIgnoreCase);
        Assert.Null(data);
    }

    // Escenario: Identificación inválida
    [Fact]
    public async Task RegistrarUsuario_IdentificacionInvalida_RetornaError()
    {
        _usuarioRepo.GetByCorreoAsync("id@test.com")
            .Returns((Usuario?)null);

        var request = BuildRequest(
            identificacion: "123");

        var (success, error, data) =
            await _sut.RegistrarUsuarioAsync(request);

        Assert.False(success);
        Assert.Contains("identificación", error!,
            StringComparison.OrdinalIgnoreCase);
        Assert.Null(data);
    }

    private static RegistroUsuarioRequest BuildRequest(
        string nombre = "Enzo Morales",
        string correo = "enzo@test.com",
        string password = "Password123",
        string tipoIdentificacion = "Fisica",
        string identificacion = "123456789")
        => new()
        {
            NombreCompleto = nombre,
            CorreoElectronico = correo,
            Contrasena = password,
            TipoIdentificacion = tipoIdentificacion,
            NumeroIdentificacion = identificacion
        };
}
