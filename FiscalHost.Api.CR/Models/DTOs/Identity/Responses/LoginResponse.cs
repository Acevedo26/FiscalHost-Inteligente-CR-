namespace FiscalHost.Api.CR.Models.DTOs.Identity.Responses;

public class LoginResponse
{
    public required string Token { get; set; }
    public required string Mensaje { get; set; }
    public required LoginUsuarioResponse Usuario { get; set; }
}

public class LoginUsuarioResponse
{
    public required string Id { get; set; }
    public required string NombreCompleto { get; set; }
    public required string CorreoElectronico { get; set; }
    public required string Rol { get; set; }
}
