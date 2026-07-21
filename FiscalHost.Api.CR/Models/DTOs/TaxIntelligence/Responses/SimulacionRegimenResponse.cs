namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

public class SimulacionRegimenResponse
{
	public decimal RentaBruta { get; set; }

	public DetalleRegimenDto CapitalInmobiliario { get; set; } = new();
	public DetalleRegimenDto Utilidades { get; set; } = new();

	public string RegimenRecomendado { get; set; } = string.Empty;
	public decimal AhorroEstimado { get; set; }
	public string Justificacion { get; set; } = string.Empty;
}

public class DetalleRegimenDto
{
	public decimal Deduccion { get; set; }
	public decimal RentaNeta { get; set; }
	public decimal ImpuestoRenta { get; set; }
	public bool CuentaConComprobantesValidos { get; set; }
}
