namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

public class SancionResponse
{
	public Guid SancionId { get; set; }
	public Guid ObligacionId { get; set; }
	public string TipoSancion { get; set; } = string.Empty;

	public decimal MontoBaseAdeudado { get; set; }
	public decimal MultaBaseCalculada { get; set; }
	public decimal PorcentajeReduccion { get; set; }
	public decimal MontoReduccion { get; set; }
	public decimal MultaReducida { get; set; }
	public decimal InteresesAcumulados { get; set; }
	public decimal MontoTotalPagar { get; set; }

	public string TipoFormularioGenerado { get; set; } = "D176";
	public string Estado { get; set; } = string.Empty;
	public string Descripcion { get; set; } = string.Empty;
}
