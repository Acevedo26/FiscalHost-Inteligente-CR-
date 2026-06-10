using FiscalHost.Api.CR.Models.DTOs;
using FiscalHost.Api.CR.Models.Entities;
using FiscalHost.Api.CR.Models.Emums;
using FiscalHost.Api.CR.Repositories;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace FiscalHost.Api.CR.Services;

public interface IAuthService
{
    Task<(bool success, string? error, RegistroUsuarioResponse? data)>
        RegistrarUsuarioAsync(RegistroUsuarioRequest request);
}

public class AuthService(IUsuarioRepository usuarioRepo) : IAuthService
{
    public async Task<(bool success, string? error, RegistroUsuarioResponse? data)>
        RegistrarUsuarioAsync(RegistroUsuarioRequest request)
    {
        var correoExistente =
            await usuarioRepo.GetByCorreoAsync(request.Correo);

        if (correoExistente is not null)
            return (false,
                "El correo electrónico ya está registrado.",
                null);

        if (!ValidarContrasena(request.Contrasena))
            return (false,
                "La contraseña no cumple las políticas de seguridad.",
                null);

        if (!ValidarIdentificacion(
                request.TipoIdentificacion,
                request.NumeroIdentificacion))
            return (false,
                "Formato de identificación inválido.",
                null);

        var usuario = new Usuario
        {
            Nombre = request.Nombre,
            Correo = request.Correo,
            ContrasenaHash = HashPassword(request.Contrasena),
            NumeroIdentificacion = request.NumeroIdentificacion,
            TipoIdentificacion =
                Enum.Parse<TipoIdentificacion>(
                    request.TipoIdentificacion,
                    true)
        };

        await usuarioRepo.AddAsync(usuario);
        await usuarioRepo.SaveChangesAsync();

        return (true,
            null,
            new RegistroUsuarioResponse
            {
                Id = usuario.Id,
                Correo = usuario.Correo,
                Mensaje = "Registro exitoso."
            });
    }

    private static bool ValidarContrasena(string password)
    {
        return password.Length >= 8
               && password.Any(char.IsUpper)
               && password.Any(char.IsDigit);
    }

    private static bool ValidarIdentificacion(
        string tipo,
        string identificacion)
    {
        return tipo.ToUpper() switch
        {
            "CEDULAFISICA" =>
                Regex.IsMatch(identificacion, @"^\d{9}$"),

            "CEDULAJURIDICA" =>
                Regex.IsMatch(identificacion, @"^\d{10}$"),

            "DIMEX" =>
                Regex.IsMatch(identificacion, @"^\d{11,12}$"),

            "NITE" =>
                Regex.IsMatch(identificacion, @"^\d{10,12}$"),

            _ => false
        };
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(password));

        return Convert.ToHexString(bytes);
    }
}