using FiscalHost.Api.CR.Repositories;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FiscalHost.Api.CR.Services;

public interface IAuthService
{
    Task<(bool success, string? error, RegistroUsuarioResponse? data)>
        RegistrarUsuarioAsync(RegistroUsuarioRequest request);

    Task<(bool success, string? error, LoginResponse? data)>
        LoginAsync(LoginRequest request);
}

public class AuthService(IUsuarioRepository usuarioRepo, IConfiguration config) : IAuthService
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

    public async Task<(bool success, string? error, LoginResponse? data)>
        LoginAsync(LoginRequest request)
    {
        var usuario = await usuarioRepo.GetByCorreoAsync(request.Correo);
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.ContrasenaHash))
        {
            return (false, "Credenciales inválidas.", null);
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not set"));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Email, usuario.CorreoElectronico)
            }),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(config["Jwt:ExpireMinutes"] ?? "15")),
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        return (true, null, new LoginResponse { Token = jwt, Mensaje = "Inicio de sesión exitoso." });
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
