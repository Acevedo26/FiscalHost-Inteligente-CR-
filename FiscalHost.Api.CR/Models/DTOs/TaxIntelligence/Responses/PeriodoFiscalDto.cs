using System;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

public class PeriodoFiscalDto
{
    public Guid PeriodoId { get; set; }
    public short Anio { get; set; }
    public short Mes { get; set; }
    public TipoFormulario TipoFormulario { get; set; }
    public DateTime FechaInicioPeriodo { get; set; }
    public DateTime FechaFinPeriodo { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public decimal TarifaIva { get; set; }
    public decimal TarifaRentaCapital { get; set; }
    public decimal DeduccionPlanaCapital { get; set; }
    public decimal? TasaInteresMoraAnual { get; set; }
    public string NormativaAplicable { get; set; } = string.Empty;
}
