using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

public class CambiarRegimenTributarioRequest
{
	[Required]
	public Guid UsuarioId { get; set; }

	[Required]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public RegimenTributario NuevoRegimen { get; set; }

	// Periodo desde el cual se confirma el cambio, para validar que existan
	// gastos reales con evidencia validada que lo justifiquen.
	[Required]
	[Range(2024, 2100)]
	public short Anio { get; set; }

	[Required]
	[Range(1, 12)]
	public short Mes { get; set; }
}
