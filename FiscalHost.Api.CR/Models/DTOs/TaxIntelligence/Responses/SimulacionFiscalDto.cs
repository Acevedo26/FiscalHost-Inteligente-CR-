using System;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

// ========================================================================
// DTO de Response (Salida): Contiene la información formateada que la API
// le devuelve al frontend para mostrar en pantalla, ocultando, de esta
// manera, cualquier dato sensible.
// ========================================================================


public class SimulacionFiscalDto
{
    public Guid SimulacionId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public short PeriodoBaseAnio { get; set; }
    public short? PeriodoBaseMes { get; set; }
    public string ParametrosEntrada { get; set; } = "{}";
    public string Resultados { get; set; } = "{}";
}


