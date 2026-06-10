using FiscalHost.Api.CR.Models.Emums;

namespace FiscalHost.Api.CR.Models.Entities;

public class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string ContrasenaHash { get; set; } = string.Empty;

    public TipoIdentificacion TipoIdentificacion { get; set; }

    public string NumeroIdentificacion { get; set; } = string.Empty;

    public bool Activo { get; set; } = false;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}