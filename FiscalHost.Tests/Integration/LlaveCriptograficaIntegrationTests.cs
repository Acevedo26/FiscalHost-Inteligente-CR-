using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.DTOs;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace FiscalHost.Tests.Integration;

public class LlaveCriptograficaIntegrationTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly LlaveCriptograficaService _service;

    public LlaveCriptograficaIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Cifrado:Clave"] = "clave-integracion-fiscalhost-2026" })
            .Build();

        var repo = new LlaveCriptograficaRepository(_db);
        var notificaciones = Substitute.For<INotificacionService>();
        _service = new LlaveCriptograficaService(repo, notificaciones, config);
    }

    [Fact]
    public async Task CargarLlave_ArchivoNoP12_NoGuardaEnDb()
    {
        var request = new CargarLlaveRequest
        {
            AnfitrionId = "ANF001",
            Archivo = CrearFormFile("cert.pfx", [0x01]),
            Contrasena = "pass"
        };

        var (success, _, _) = await _service.CargarLlaveAsync(request);

        Assert.False(success);
        Assert.Empty(_db.LlavesCriptograficas);
    }

    [Fact]
    public async Task CargarLlave_CertificadoInvalidoP12_NoGuardaEnDb()
    {
        var request = new CargarLlaveRequest
        {
            AnfitrionId = "ANF001",
            Archivo = CrearFormFile("cert.p12", [0x00, 0x01, 0x02]),
            Contrasena = "pass"
        };

        var (success, error, _) = await _service.CargarLlaveAsync(request);

        Assert.False(success);
        Assert.NotNull(error);
        Assert.Empty(_db.LlavesCriptograficas);
    }

    [Fact]
    public async Task GetLlave_DespuesDeCargarInvalido_RetornaNull()
    {
        var result = await _service.GetLlaveAsync("ANF_NUEVO");
        Assert.Null(result);
    }

    public void Dispose() => _db.Dispose();

    private static IFormFile CrearFormFile(string nombre, byte[] contenido)
    {
        var stream = new MemoryStream(contenido);
        return new FormFile(stream, 0, contenido.Length, "Archivo", nombre);
    }
}
