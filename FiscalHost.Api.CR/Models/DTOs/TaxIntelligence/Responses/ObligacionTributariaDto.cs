using System;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

public class ObligacionTributariaDto
{
    public Guid ObligacionId { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid? CalculoId { get; set; }
    public Guid PeriodoId { get; set; }
    public TipoFormulario TipoFormulario { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal MontoCapital { get; set; }
    public decimal MontoMulta { get; set; }
    public decimal MontoInteresesAcumulados { get; set; }
    public decimal MontoTotalActualizado { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public DateTime? FechaPago { get; set; }
    public EstadoObligacion Estado { get; set; }
    public decimal? TasaInteresAplicada { get; set; }
    public string HistorialIntereses { get; set; } = "{}";
}
