using System;
using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

public class CreateSimulacionFiscalRequest
{
    [Required]
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    [Required]
    public short PeriodoBaseAnio { get; set; }
    public short? PeriodoBaseMes { get; set; }
    
    [Required]
    public SimulacionParametrosDto Parametros { get; set; } = new SimulacionParametrosDto();
}
