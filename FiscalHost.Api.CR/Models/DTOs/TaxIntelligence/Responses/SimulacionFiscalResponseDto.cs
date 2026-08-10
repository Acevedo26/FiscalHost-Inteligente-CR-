using System;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

public class SimulacionFiscalResponseDto
{
    public Guid SimulacionId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public short PeriodoBaseAnio { get; set; }
    public short? PeriodoBaseMes { get; set; }
    public SimulacionParametrosDto Parametros { get; set; } = new SimulacionParametrosDto();
    public SimulacionResultadosDto Resultados { get; set; } = new SimulacionResultadosDto();
}
