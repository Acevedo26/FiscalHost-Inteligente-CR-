using System;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence;

public class SimulacionResultadosDto
{
    public decimal IvaEstimado { get; set; }
    public decimal RentaEstimada { get; set; }
    public decimal TotalImpuestosEstimados { get; set; }
    public decimal AhorroFiscalEsperado { get; set; }
}
