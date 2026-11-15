using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace FiscalHost.Tests.Unit;

public class LocalStorageServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock = new();
    private readonly Mock<IWebHostEnvironment> _environmentMock = new();
    private readonly string _testBasePath;
    private readonly FiscalHost.Api.CR.Services.LocalStorageService _service;

    public LocalStorageServiceTests()
    {
        // Usar path en temp con nombre fijo para evitar problemas con temp
        _testBasePath = Path.Combine(Path.GetTempPath(), "FiscalHostTests_LocalStorage");
        var created = Directory.CreateDirectory(_testBasePath);
        
        // Clave de 32 bytes (256 bits) para AES-256
        var testKey = "clave-secreta-minimo-32-caracteres!!";
        _configurationMock.Setup(x => x["LocalStorage:UploadPath"]).Returns("uploads/comprobantes");
        _configurationMock.Setup(x => x["Cifrado:Clave"]).Returns(testKey);
        _environmentMock.Setup(x => x.ContentRootPath).Returns(_testBasePath);

        _service = new FiscalHost.Api.CR.Services.LocalStorageService(_configurationMock.Object, _environmentMock.Object);
    }
    
    [Fact]
    public void Constructor_CalculaBasePathCorrectamente()
    {
        // Verificar que el mock devuelve la clave correctamente
        var clave = _configurationMock.Object["Cifrado:Clave"];
        Assert.Equal("clave-secreta-minimo-32-caracteres!!", clave);
        
        var uploadPath = _configurationMock.Object["LocalStorage:UploadPath"];
        Assert.Equal("uploads/comprobantes", uploadPath);
        
        var contentRoot = _environmentMock.Object.ContentRootPath;
        Assert.Equal(_testBasePath, contentRoot);
    }

    [Fact]
    public async Task UploadAsync_CifraArchivoYDescifraCorrectamente()
    {
        // Arrange
        var contenidoOriginal = "Contenido sensible del comprobante - factura #12345";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(contenidoOriginal));

        // Act - usar el servicio directamente
        string url;
        try
        {
            url = await _service.UploadAsync(stream, "factura.pdf", "application/pdf");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Excepción en UploadAsync: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            return;
        }
        
        // Assert: URL contiene .enc
        Assert.Contains(".enc", url);
        
        // El archivo se guarda en _testBasePath/uploads/comprobantes/
        var uploadsDir = Path.Combine(_testBasePath, "uploads", "comprobantes");
        var fileName = Path.GetFileName(url);
        var filePath = Path.Combine(uploadsDir, fileName);
        
        // Debug info
        var dirExists = Directory.Exists(uploadsDir);
        var files = dirExists ? Directory.GetFiles(uploadsDir) : [];
        var testBaseExists = Directory.Exists(_testBasePath);
        var basePathExpected = Path.Combine(_testBasePath, "uploads", "comprobantes");
        
        Assert.True(File.Exists(filePath), 
            $"Archivo no existe en: {filePath}\n" +
            $"_testBasePath: {_testBasePath} (existe: {testBaseExists})\n" +
            $"basePath esperado: {basePathExpected}\n" +
            $"uploadsDir: {uploadsDir} (existe: {dirExists})\n" +
            $"Archivos en dir: {string.Join(", ", files)}");
        
        // Verificar que el archivo en disco NO contiene el texto original (está cifrado)
        var bytesEnDisco = await File.ReadAllBytesAsync(filePath);
        var textoEnDisco = System.Text.Encoding.UTF8.GetString(bytesEnDisco);
        Assert.DoesNotContain(contenidoOriginal, textoEnDisco);
        
        // Descifrar y verificar que coincide
        var clave = "clave-secreta-minimo-32-caracteres!!";
        var descifrado = DescifrarArchivo(filePath, clave);
        Assert.Equal(contenidoOriginal, descifrado);
    }
    
    [Fact]
    public async Task UploadAsync_SinClaveConfigurada_LanzaExcepcion()
    {
        // Arrange
        var configSinClave = new Mock<IConfiguration>();
        configSinClave.Setup(x => x["LocalStorage:UploadPath"]).Returns("uploads/comprobantes");
        configSinClave.Setup(x => x["Cifrado:Clave"]).Returns((string?)null);
        
        var serviceSinClave = new FiscalHost.Api.CR.Services.LocalStorageService(configSinClave.Object, _environmentMock.Object);
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("test"));
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => serviceSinClave.UploadAsync(stream, "test.pdf", "application/pdf"));
    }

    [Fact]
    public async Task UploadAsync_DosArchivosMismoContenido_TienenIVsDistintos()
    {
        // Arrange: mismo contenido, dos uploads
        var contenido = "Contenido idéntico para probar IV único";
        var stream1 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(contenido));
        var stream2 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(contenido));
        
        // Act
        var url1 = await _service.UploadAsync(stream1, "a.pdf", "application/pdf");
        var url2 = await _service.UploadAsync(stream2, "b.pdf", "application/pdf");
        
        // Assert: URLs distintas (Guid en nombre)
        Assert.NotEqual(url1, url2);
        
        // Leer IVs de ambos archivos (primeros 16 bytes)
        var uploadsDir = Path.Combine(_testBasePath, "uploads", "comprobantes");
        var file1 = Path.Combine(uploadsDir, Path.GetFileName(url1));
        var file2 = Path.Combine(uploadsDir, Path.GetFileName(url2));
        
        var iv1 = (await File.ReadAllBytesAsync(file1)).Take(16).ToArray();
        var iv2 = (await File.ReadAllBytesAsync(file2)).Take(16).ToArray();
        
        // IVs deben ser distintos (aleatorios criptográficamente)
        Assert.NotEqual(iv1, iv2);
        
        // Pero ambos deben descifrar al mismo contenido original
        var clave = "clave-secreta-minimo-32-caracteres!!";
        var descifrado1 = DescifrarArchivo(file1, clave);
        var descifrado2 = DescifrarArchivo(file2, clave);
        Assert.Equal(contenido, descifrado1);
        Assert.Equal(contenido, descifrado2);
    }

    private static string DescifrarArchivo(string filePath, string clave)
    {
        var bytes = File.ReadAllBytes(filePath);
        
        // Formato: [IV:16 bytes] + [EncryptedData]
        var iv = bytes.Take(16).ToArray();
        var encryptedData = bytes.Skip(16).ToArray();

        using var aes = Aes.Create();
        aes.Key = System.Text.Encoding.UTF8.GetBytes(clave[..32]); // 32 bytes = 256 bits
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(encryptedData);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }
}