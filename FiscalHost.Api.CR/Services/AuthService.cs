using FiscalHost.Api.CR.Repositories;
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
            await usuarioRepo.GetByCorreoAsync(request.CorreoElectronico);

        if (correoExistente is not null)
            return (false,
                "El correo electrónico ya está registrado.",
                null);

        var identificacionExistente =
            await usuarioRepo.GetByIdentificacionAsync(request.NumeroIdentificacion);

        if (identificacionExistente is not null)
            return (false,
                "El número de identificación ya está registrado.",
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
            NombreCompleto = request.NombreCompleto,
            CorreoElectronico = request.CorreoElectronico,
            ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(request.Contrasena),
            NumeroIdentificacion = request.NumeroIdentificacion,
            RazonSocial = request.RazonSocial,
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
                UsuarioId = usuario.UsuarioId,
                CorreoElectronico = usuario.CorreoElectronico,
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
            "FISICA" =>
                Regex.IsMatch(identificacion, @"^\d{9}$"),

            "JURIDICA" =>
                Regex.IsMatch(identificacion, @"^\d{10}$"),

            "DIMEX" =>
                Regex.IsMatch(identificacion, @"^\d{11,12}$"),

            "NITE" =>
                Regex.IsMatch(identificacion, @"^\d{10,12}$"),

            _ => false
        };
    }

}
