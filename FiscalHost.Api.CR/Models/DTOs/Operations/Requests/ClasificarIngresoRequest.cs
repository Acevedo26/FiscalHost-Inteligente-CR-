using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Requests;

public class ClasificarIngresoRequest
{
    [Required]
    public string AnfitrionId { get; set; } = string.Empty;

    [Required]
    public DateTime FechaEntrada { get; set; }

    [Required]
    public DateTime FechaSalida { get; set; }

    [Required]
    public decimal MontoBruto { get; set; }

    [Required]
    public FuenteIngreso FuenteIngreso { get; set; } = FuenteIngreso.Nacional;

    public bool TieneFacturaElectronicaNacional { get; set; }

    public bool HuespedResidente { get; set; } = true;
}
