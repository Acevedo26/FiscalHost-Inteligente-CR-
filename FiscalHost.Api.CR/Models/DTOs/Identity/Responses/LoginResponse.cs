namespace FiscalHost.Api.CR.Models.DTOs.Identity.Responses;

public class LoginResponse
{
    public required string Token { get; set; }
    public required string Mensaje { get; set; }
}
