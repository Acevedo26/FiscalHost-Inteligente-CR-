using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Requests;

// ========================================================================
// DTO de Request (Entrada): Contiene, únicamente, los datos que el usuario
// o frontend envía a la API al realizar una petición, sin incluir
// identificadores generados ni campos de auditoría internos.
// ========================================================================


public class ReservaDirectaRequest
{
    [JsonIgnore]
    public string AnfitrionId { get; set; } = string.Empty;

    [Required]
    public DateTime FechaReserva { get; set; }

    [Required]
    public decimal Monto { get; set; }

    [Required]
    public string Huesped { get; set; } = string.Empty;
}


