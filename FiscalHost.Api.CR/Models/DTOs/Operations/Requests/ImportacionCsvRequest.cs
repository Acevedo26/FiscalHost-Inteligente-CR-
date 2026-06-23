namespace FiscalHost.Api.CR.Models.DTOs.Operations.Requests;

public class ImportacionCsvRequest
{
	public DateTime FechaInicio { get; set; }

	public DateTime FechaFin { get; set; }

	public decimal MontoBruto { get; set; }

	public PlataformaOrigen Plataforma { get; set; }

	public string IdReserva { get; set; } = string.Empty;
}