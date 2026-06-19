using System;
using System.Collections.Generic;

namespace FiscalHost.Api.CR.Models.Entities.Audit;

public class LlaveCriptografica
{
    public int Id { get; set; }
    public string AnfitrionId { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public byte[] ContenidoCifrado { get; set; } = [];
    public string ContrasenaHash { get; set; } = string.Empty;  // AES-256 cifrada
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    public bool Activa { get; set; } = true;
    public ICollection<AuditoriaLlave> Auditorias { get; set; } = [];
}
