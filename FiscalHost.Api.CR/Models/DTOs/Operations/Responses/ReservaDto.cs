using System;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Responses;

// ========================================================================
// DTO de Response (Salida): Contiene la información formateada que la API
// le devuelve al frontend para mostrar en pantalla, ocultando, de esta
// manera, cualquier dato sensible.
// ========================================================================


public class ReservaDto
{
    public Guid ReservaId { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid? PropiedadId { get; set; }
    public Guid? ImportacionId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string? NombreHuesped { get; set; }
    public string? IdentificacionHuesped { get; set; }
    public string? PaisOrigenHuesped { get; set; }
    public decimal MontoBruto { get; set; }
    public TipoMoneda Moneda { get; set; }
    public decimal TipoCambio { get; set; }
    public decimal MontoColones { get; set; }
    public ClasificacionFiscal ClasificacionFiscal { get; set; }
    public decimal MontoGravado { get; set; }
    public decimal MontoExento { get; set; }
    public decimal MontoIvaCalculado { get; set; }
    public decimal RetencionExtranjera { get; set; }
    public PlataformaOrigen PlataformaOrigen { get; set; }
    public FuenteRegistro FuenteRegistro { get; set; }
    public string? ReferenciaPlataforma { get; set; }
    public bool FueReclasificada { get; set; }
    public string? JustificacionReclasificacion { get; set; }
    public DateTimeOffset? FechaReclasificacion { get; set; }
    public Guid? UsuarioReclasificacionId { get; set; }
    public short PeriodoFiscalAnio { get; set; }
    public short PeriodoFiscalMes { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Metadata { get; set; } = "{}";
}


