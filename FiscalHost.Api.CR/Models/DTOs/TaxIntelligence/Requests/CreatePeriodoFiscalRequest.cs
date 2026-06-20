using System;
using System.ComponentModel.DataAnnotations;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

// ========================================================================
// DTO de Request (Entrada): Contiene, únicamente, los datos que el usuario
// o frontend envía a la API al realizar una petición, sin incluir
// identificadores generados ni campos de auditoría internos.
// ========================================================================


public class CreatePeriodoFiscalRequest
{
    [Required]
    public short Anio { get; set; }

    [Required]
    public short Mes { get; set; }

    [Required]
    public TipoFormulario TipoFormulario { get; set; }

    [Required]
    public DateTime FechaInicioPeriodo { get; set; }

    [Required]
    public DateTime FechaFinPeriodo { get; set; }

    [Required]
    public DateTime FechaVencimiento { get; set; }

    [Required]
    public decimal TarifaIva { get; set; }

    [Required]
    public decimal TarifaRentaCapital { get; set; }

    [Required]
    public decimal DeduccionPlanaCapital { get; set; }

    public decimal? TasaInteresMoraAnual { get; set; }

    public string NormativaAplicable { get; set; } = string.Empty;
}


