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

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        Directory.CreateDirectory(_basePath);

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        var filePath = Path.Combine(_basePath, uniqueFileName);

        using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await fileStream.CopyToAsync(fs);

        return $"/uploads/comprobantes/{uniqueFileName}";
    }
}
