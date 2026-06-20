using System;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Responses;

// ========================================================================
// DTO de Response (Salida): Contiene la información formateada que la API
// le devuelve al frontend para mostrar en pantalla, ocultando, de esta
// manera, cualquier dato sensible.
// ========================================================================


public class PropiedadDto
{
    public Guid PropiedadId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string? Provincia { get; set; }
    public string? Canton { get; set; }
    public string? Distrito { get; set; }
    public string? NumeroFinca { get; set; }
    public decimal? ValorFiscal { get; set; }
    public TipoMoneda? TipoMonedaValor { get; set; }
    public bool Activa { get; set; }
}


