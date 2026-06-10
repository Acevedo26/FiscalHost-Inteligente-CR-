using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using FiscalHost.Api.CR.Models.DTOs;
using FiscalHost.Api.CR.Models.Entities;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public interface INotificacionService
{
    Task NotificarAsync(string anfitrionId, string mensaje);
}

public interface ILlaveCriptograficaService
{
    Task<(bool success, string? error, LlaveCriptograficaResponse? data)> CargarLlaveAsync(CargarLlaveRequest request);
    Task<(bool success, string? error)> ActualizarContrasenaAsync(ActualizarContrasenaRequest request);
    Task<LlaveCriptograficaResponse?> GetLlaveAsync(string anfitrionId);
}

public class LlaveCriptograficaService(
    ILlaveCriptograficaRepository repo,
    INotificacionService notificaciones,
    IConfiguration config) : ILlaveCriptograficaService
{
    public async Task<(bool success, string? error, LlaveCriptograficaResponse? data)> CargarLlaveAsync(CargarLlaveRequest request)
    {
        if (!request.Archivo.FileName.EndsWith(".p12", StringComparison.OrdinalIgnoreCase))
            return (false, "Solo se permiten archivos con extensión .p12.", null);

        using var ms = new MemoryStream();
        await request.Archivo.CopyToAsync(ms);
        var bytes = ms.ToArray();

        if (!ValidarCertificado(bytes, request.Contrasena, out var errorCert))
            return (false, errorCert, null);

        var existing = await repo.GetByAnfitrionIdAsync(request.AnfitrionId);
        var contrasenacifrada = HashContrasena(request.Contrasena);
        var contenidoCifrado = CifrarBytes(bytes);

        if (existing is null)
        {
            existing = new LlaveCriptografica
            {
                AnfitrionId = request.AnfitrionId,
                NombreArchivo = request.Archivo.FileName,
                ContenidoCifrado = contenidoCifrado,
                ContrasenaHash = contrasenacifrada
            };
            await repo.AddAsync(existing);
        }
        else
        {
            existing.NombreArchivo = request.Archivo.FileName;
            existing.ContenidoCifrado = contenidoCifrado;
            existing.ContrasenaHash = contrasenacifrada;
            existing.FechaActualizacion = DateTime.UtcNow;
        }

        await repo.SaveChangesAsync();
        await RegistrarAuditoriaAsync(existing.Id, "CARGA", "Llave criptográfica almacenada exitosamente.");
        return (true, null, MapToResponse(existing));
    }

    public async Task<(bool success, string? error)> ActualizarContrasenaAsync(ActualizarContrasenaRequest request)
    {
        var llave = await repo.GetByAnfitrionIdAsync(request.AnfitrionId);
        if (llave is null) return (false, "No se encontró una llave activa para el anfitrión.");

        if (llave.ContrasenaHash != HashContrasena(request.ContrasenaActual))
            return (false, "La contraseña actual es incorrecta.");

        var contenidoDescifrado = DescifrarBytes(llave.ContenidoCifrado);
        if (!ValidarCertificado(contenidoDescifrado, request.ContrasenaNueva, out var errorCert))
            return (false, errorCert);

        llave.ContrasenaHash = HashContrasena(request.ContrasenaNueva);
        llave.FechaActualizacion = DateTime.UtcNow;
        await repo.SaveChangesAsync();

        await RegistrarAuditoriaAsync(llave.Id, "ACTUALIZAR_CONTRASENA", "Contraseña de llave criptográfica actualizada.");
        await notificaciones.NotificarAsync(request.AnfitrionId, "La contraseña de su llave criptográfica ha sido actualizada.");
        return (true, null);
    }

    public async Task<LlaveCriptograficaResponse?> GetLlaveAsync(string anfitrionId)
    {
        var llave = await repo.GetByAnfitrionIdAsync(anfitrionId);
        return llave is null ? null : MapToResponse(llave);
    }

    private static bool ValidarCertificado(byte[] bytes, string contrasena, out string? error)
    {
        try
        {
            using var cert = new X509Certificate2(bytes, contrasena);
            error = null;
            return true;
        }
        catch (CryptographicException ex) when (ex.Message.Contains("password", StringComparison.OrdinalIgnoreCase)
                                                 || ex.HResult == unchecked((int)0x80090056))
        {
            error = "La contraseña proporcionada es incorrecta.";
            return false;
        }
        catch
        {
            error = "El archivo no es un certificado .p12 válido.";
            return false;
        }
    }

    private async Task RegistrarAuditoriaAsync(int llaveId, string accion, string descripcion)
    {
        await repo.AddAuditoriaAsync(new AuditoriaLlave
        {
            LlaveCriptograficaId = llaveId,
            Accion = accion,
            Descripcion = descripcion
        });
        await repo.SaveChangesAsync();
    }

    private byte[] GetClaveAes()
    {
        var clave = config["Cifrado:Clave"] ?? throw new InvalidOperationException("Cifrado:Clave no configurado.");
        return SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(clave));
    }

    // HMAC-SHA256 es determinístico: misma clave + mismo texto = mismo hash, apto para comparación.
    private string HashContrasena(string texto)
    {
        var clave = System.Text.Encoding.UTF8.GetBytes(
            config["Cifrado:Clave"] ?? throw new InvalidOperationException("Cifrado:Clave no configurado."));
        var hash = HMACSHA256.HashData(clave, System.Text.Encoding.UTF8.GetBytes(texto));
        return Convert.ToBase64String(hash);
    }

    private byte[] CifrarBytes(byte[] datos)
    {
        using var aes = Aes.Create();
        aes.Key = GetClaveAes();
        aes.GenerateIV();
        using var encryptor = aes.CreateEncryptor();
        var cifrado = encryptor.TransformFinalBlock(datos, 0, datos.Length);
        return [.. aes.IV, .. cifrado];
    }

    private byte[] DescifrarBytes(byte[] datos)
    {
        using var aes = Aes.Create();
        aes.Key = GetClaveAes();
        aes.IV = datos[..16];
        using var decryptor = aes.CreateDecryptor();
        return decryptor.TransformFinalBlock(datos, 16, datos.Length - 16);
    }

    private static LlaveCriptograficaResponse MapToResponse(LlaveCriptografica l) => new()
    {
        Id = l.Id,
        AnfitrionId = l.AnfitrionId,
        NombreArchivo = l.NombreArchivo,
        FechaActualizacion = l.FechaActualizacion,
        Activa = l.Activa
    };
}
