using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace FiscalHost.Api.CR.Services;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
}

// ========================================================================
// Servicio de Almacenamiento Seguro: Sube comprobantes a Azure Blob Storage.
// Se implementa cumpliendo el requerimiento de la HU-007 sobre 
// almacenamiento seguro. Azure Blob Storage utiliza cifrado AES-256 en 
// reposo (SSE) de manera predeterminada para todos los datos.
// ========================================================================
public class BlobStorageService : IBlobStorageService
{
    private readonly string _connectionString;
    private readonly string _containerName;

    public BlobStorageService(IConfiguration configuration)
    {
        // Se espera que la configuración tenga las credenciales de Azure
        _connectionString = configuration.GetConnectionString("AzureBlobStorage") ?? "UseDevelopmentStorage=true";
        _containerName = configuration["Azure:BlobContainerName"] ?? "comprobantes-gastos";
    }

    /// <summary>
    /// Sube un archivo de forma segura a Azure Blob Storage.
    /// </summary>
    /// <param name="fileStream">Flujo de datos del archivo (imagen o pdf).</param>
    /// <param name="fileName">Nombre del archivo original.</param>
    /// <param name="contentType">Tipo MIME del archivo.</param>
    /// <returns>La URL del blob subido de manera segura.</returns>
    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        // 1. Crear el cliente del servicio Blob apuntando a la cuenta de almacenamiento
        var blobServiceClient = new BlobServiceClient(_connectionString);
        
        // 2. Obtener la referencia al contenedor donde se guardan los comprobantes
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        
        // 3. Si el contenedor no existe, se crea automáticamente
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        // 4. Generar un nombre único para el archivo para evitar colisiones, usando GUID
        string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        var blobClient = containerClient.GetBlobClient(uniqueFileName);

        // 5. Configurar las opciones de subida con el tipo MIME correcto
        var uploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        // 6. Ejecutar la subida del archivo al almacenamiento de Azure.
        // Nota: Al alojarse en Azure Blob Storage, los datos se cifran automáticamente en reposo (AES-256).
        await blobClient.UploadAsync(fileStream, uploadOptions);

        // 7. Retornar la URL generada del blob para ser guardada en la base de datos de PostgreSQL
        return blobClient.Uri.ToString();
    }
}
