namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

public class RentaCapitalResponse
{
	public Guid CalculoId { get; set; }
	public short Anio { get; set; }
	public short Mes { get; set; }
	public string RegimenAplicado { get; set; } = string.Empty;

	public decimal RentaBruta { get; set; }
	public decimal DeduccionAplicada { get; set; }
	public decimal RentaNeta { get; set; }
	public decimal TasaEfectiva { get; set; }
	public decimal ImpuestoRenta { get; set; }
	public decimal RetencionesAcreditadas { get; set; }
	public decimal MontoTotalAPagar { get; set; }

	public bool BorradorGenerado { get; set; }
	public DateTimeOffset? FechaGeneracionBorrador { get; set; }
}
