using System;
using System.Collections.Generic;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

public class ComparacionSimulacionesResponseDto
{
    public List<SimulacionFiscalResponseDto> Simulaciones { get; set; } = new List<SimulacionFiscalResponseDto>();
    public List<string> Advertencias { get; set; } = new List<string>();
}
