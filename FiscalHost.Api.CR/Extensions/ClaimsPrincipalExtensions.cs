using System.Security.Claims;

namespace FiscalHost.Api.CR.Extensions;

// Extraer el ID de un usuario autenticado a partir de su Token (JWT)
public static class ClaimsPrincipalExtensions
{
    public static Guid GetUsuarioId(this ClaimsPrincipal user)
    {
        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            throw new UnauthorizedAccessException("El token es inválido o no contiene el ID del usuario.");
        }
        return userId;
    }
}
