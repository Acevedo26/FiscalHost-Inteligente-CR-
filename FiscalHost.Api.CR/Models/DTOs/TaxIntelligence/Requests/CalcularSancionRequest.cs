using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

public class CalcularSancionRequest
{
	[Required]
	public Guid UsuarioId { get; set; }

	// Obligacion tributaria pendiente sobre la cual se autoliquida la sancion.
	[Required]
	public Guid ObligacionId { get; set; }
}
