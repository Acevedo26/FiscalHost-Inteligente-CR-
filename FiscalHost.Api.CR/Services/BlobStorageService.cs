using System.Security.Cryptography;

namespace FiscalHost.Api.CR.Services;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
}

public class LocalStorageService(IConfiguration configuration, IWebHostEnvironment env) : IBlobStorageService
{
    private readonly string _basePath = Path.Combine(
        env.ContentRootPath,
        configuration["LocalStorage:UploadPath"] ?? "uploads/comprobantes");

    private readonly string? _encryptionKey = configuration["Cifrado:Clave"];

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        if (string.IsNullOrEmpty(_encryptionKey) || _encryptionKey.Length < 32)
            throw new InvalidOperationException("Clave de cifrado no configurada o inválida (mínimo 32 caracteres).");

        Directory.CreateDirectory(_basePath);

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}.enc";
        var filePath = Path.Combine(_basePath, uniqueFileName);

        // Generar IV aleatorio
        var iv = RandomNumberGenerator.GetBytes(16);

        using var aes = Aes.Create();
        aes.Key = System.Text.Encoding.UTF8.GetBytes(_encryptionKey[..32]);
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        // Escribir IV al inicio del archivo
        await fs.WriteAsync(iv);
        
        using var encryptor = aes.CreateEncryptor();
        using var cs = new CryptoStream(fs, encryptor, CryptoStreamMode.Write);
        await fileStream.CopyToAsync(cs);
        await cs.FlushFinalBlockAsync();

        return $"/uploads/comprobantes/{uniqueFileName}";
    }
}
