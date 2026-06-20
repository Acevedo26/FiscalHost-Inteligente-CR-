using System;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

// ========================================================================
// DTO de Response (Salida): Contiene la información formateada que la API
// le devuelve al frontend para mostrar en pantalla, ocultando, de esta
// manera, cualquier dato sensible.
// ========================================================================


public class CatalogoActividadEconomicaDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Seccion { get; set; }
    public decimal TarifaIva { get; set; }
    public bool Vigente { get; set; }
    public DateTime FechaVigenciaDesde { get; set; }
    public DateTime? FechaVigenciaHasta { get; set; }
}


