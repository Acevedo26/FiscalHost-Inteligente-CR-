namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

public class ReconstruccionBaseImponibleResponse
{
    public bool Success { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public short AnioFiscal { get; set; }
    public List<int> MesesSinDatos { get; set; } = [];
    public List<int> MesesSinNormativa { get; set; } = [];
    public List<ReconstruccionMensualDto> BasesMensuales { get; set; } = [];
    public ReconstruccionConsolidadoDto Consolidado { get; set; } = new();
}

public class ReconstruccionMensualDto
{
    public int Mes { get; set; }
    public bool TieneDatos { get; set; }
    public bool TieneNormativaHistorica { get; set; }
    public decimal TarifaIvaAplicada { get; set; }
    public decimal TarifaRentaAplicada { get; set; }
    public decimal DeduccionAplicada { get; set; }
    public decimal IngresosBrutos { get; set; }
    public decimal IngresosGravados { get; set; }
    public decimal IngresosExentos { get; set; }
    public decimal DebitoFiscal { get; set; }
    public decimal RetencionesAcreditadas { get; set; }
    public decimal BaseImponibleRenta { get; set; }
    public decimal ImpuestoRenta { get; set; }
    public decimal TotalAPagar { get; set; }
}

public class ReconstruccionConsolidadoDto
{
    public decimal TotalIngresosBrutos { get; set; }
    public decimal TotalIngresosGravados { get; set; }
    public decimal TotalIngresosExentos { get; set; }
    public decimal TotalDebitoFiscal { get; set; }
    public decimal TotalRetenciones { get; set; }
    public decimal TotalBaseImponibleRenta { get; set; }
    public decimal TotalImpuestoRenta { get; set; }
    public decimal TotalAPagar { get; set; }
}

public class ValidacionHistoricoResponse
{
    public bool Success { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public List<string> ColumnasFaltantes { get; set; } = [];
    public string PlantillaReferenciaUrl { get; set; } = "/api/reconstrucciones-bases/plantilla";
}
