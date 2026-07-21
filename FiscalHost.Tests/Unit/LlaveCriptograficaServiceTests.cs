using FiscalHost.Api.CR.Models.DTOs;
using FiscalHost.Api.CR.Models.Entities;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace FiscalHost.Tests.Unit;

public class LlaveCriptograficaServiceTests
{
    private readonly ILlaveCriptograficaRepository _repo = Substitute.For<ILlaveCriptograficaRepository>();
    private readonly INotificacionService _notificaciones = Substitute.For<INotificacionService>();
    private readonly IConfiguration _config;
    private readonly LlaveCriptograficaService _service;

    public LlaveCriptograficaServiceTests()
    {
        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Cifrado:Clave"] = "clave-test-fiscalhost-segura-2026" })
            .Build();
        _service = new LlaveCriptograficaService(_repo, _notificaciones, _config);
    }

    [Fact]
    public async Task CargarLlave_ArchivoNoP12_RetornaError()
    {
        var archivo = CrearFormFile("llave.pfx", [0x01]);
        var request = new CargarLlaveRequest { AnfitrionId = "ANF001", Archivo = archivo, Contrasena = "pass" };

        var (success, error, _) = await _service.CargarLlaveAsync(request);

        Assert.False(success);
        Assert.Contains(".p12", error);
    }

    [Fact]
    public async Task CargarLlave_CertificadoInvalido_RetornaError()
    {
        var archivo = CrearFormFile("llave.p12", [0x01, 0x02, 0x03]);
        var request = new CargarLlaveRequest { AnfitrionId = "ANF001", Archivo = archivo, Contrasena = "pass" };

        var (success, error, _) = await _service.CargarLlaveAsync(request);

        Assert.False(success);
        Assert.NotNull(error);
    }

    [Fact]
    public async Task ActualizarContrasena_ContrasenaActualIncorrecta_RetornaError()
    {
        // El hash almacenado corresponde a "contrasena-correcta", no a "contrasena-erronea"
        var claveBytes = System.Text.Encoding.UTF8.GetBytes("clave-test-fiscalhost-segura-2026");
        var hashCorrecto = Convert.ToBase64String(
            System.Security.Cryptography.HMACSHA256.HashData(
                claveBytes,
                System.Text.Encoding.UTF8.GetBytes("contrasena-correcta")));

        var llave = new LlaveCriptografica
        {
            Id = 1, AnfitrionId = "ANF001", NombreArchivo = "llave.p12",
            ContenidoCifrado = [], ContrasenaHash = hashCorrecto, Activa = true
        };
        _repo.GetByAnfitrionIdAsync("ANF001").Returns(llave);

        var request = new ActualizarContrasenaRequest
        {
            AnfitrionId = "ANF001",
            ContrasenaActual = "contrasena-erronea",
            ContrasenaNueva = "nueva"
        };

        var (success, error) = await _service.ActualizarContrasenaAsync(request);

        Assert.False(success);
        Assert.Contains("incorrecta", error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ActualizarContrasena_AnfitrionNoExiste_RetornaError()
    {
        _repo.GetByAnfitrionIdAsync("ANF999").Returns((LlaveCriptografica?)null);
        var request = new ActualizarContrasenaRequest
        {
            AnfitrionId = "ANF999",
            ContrasenaActual = "vieja",
            ContrasenaNueva = "nueva"
        };

        var (success, error) = await _service.ActualizarContrasenaAsync(request);

        Assert.False(success);
        Assert.Contains("no se encontró", error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetLlave_AnfitrionSinLlave_RetornaNull()
    {
        _repo.GetByAnfitrionIdAsync("ANF000").Returns((LlaveCriptografica?)null);

        var result = await _service.GetLlaveAsync("ANF000");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetLlave_AnfitrionConLlave_RetornaDatos()
    {
        var id = Guid.NewGuid();
        var idStr = id.ToString();
        var llave = new LlaveCriptografica
        {
            Id = 1, UsuarioId = id, NombreArchivo = "llave.p12",
            ContenidoCifrado = [], ContrasenaHash = "hash", Activa = true
        };
        _repo.GetByAnfitrionIdAsync(idStr).Returns(llave);

        var result = await _service.GetLlaveAsync(idStr);

        Assert.NotNull(result);
        Assert.Equal(idStr, result.AnfitrionId);
    }

    private static IFormFile CrearFormFile(string nombre, byte[] contenido)
    {
        var stream = new MemoryStream(contenido);
        return new FormFile(stream, 0, contenido.Length, "Archivo", nombre);
    }
}
