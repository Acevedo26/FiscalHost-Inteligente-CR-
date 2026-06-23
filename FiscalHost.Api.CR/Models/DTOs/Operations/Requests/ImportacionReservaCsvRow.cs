namespace FiscalHost.Api.CR.Models.DTOs.Operations.Requests;

public class ImportacionReservaCsvRow
{
	public string? FechaInicio { get; set; }
	public string? FechaFin { get; set; }
	public string? MontoBruto { get; set; }
	public string? PlataformaOrigen { get; set; }
	public string? ReferenciaPlataforma { get; set; }
}