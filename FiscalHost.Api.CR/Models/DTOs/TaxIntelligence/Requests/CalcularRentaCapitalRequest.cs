using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

public class CalcularRentaCapitalRequest
{
	[Required]
	public Guid UsuarioId { get; set; }

	[Required]
	[Range(2024, 2100)]
	public short Anio { get; set; }

	[Required]
	[Range(1, 12)]
	public short Mes { get; set; }
}
