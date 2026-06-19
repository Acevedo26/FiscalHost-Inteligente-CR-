using System;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

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
