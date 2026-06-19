using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Requests;

public class ReservaDirectaRequest
{
    [Required]
    public string AnfitrionId { get; set; } = string.Empty;

    [Required]
    public DateTime FechaReserva { get; set; }

    [Required]
    public decimal Monto { get; set; }

    [Required]
    public string Huesped { get; set; } = string.Empty;
}
