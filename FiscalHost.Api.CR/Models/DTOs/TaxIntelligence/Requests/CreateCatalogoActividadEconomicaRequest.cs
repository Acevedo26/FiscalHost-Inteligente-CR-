using System;
using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

public class CreateCatalogoActividadEconomicaRequest
{
    [Required]
    [MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    public string Descripcion { get; set; } = string.Empty;

    public string? Seccion { get; set; }

    [Required]
    public decimal TarifaIva { get; set; }

    [Required]
    public DateTime FechaVigenciaDesde { get; set; }

    public DateTime? FechaVigenciaHasta { get; set; }
}
