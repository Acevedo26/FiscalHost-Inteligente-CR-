using System;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

public class SimulacionFiscalDto
{
    public Guid SimulacionId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public short PeriodoBaseAnio { get; set; }
    public short? PeriodoBaseMes { get; set; }
    public string ParametrosEntrada { get; set; } = "{}";
    public string Resultados { get; set; } = "{}";
}
