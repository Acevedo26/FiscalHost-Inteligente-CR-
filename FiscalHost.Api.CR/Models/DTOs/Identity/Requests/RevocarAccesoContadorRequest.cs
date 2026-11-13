using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Requests;

public class RevocarAccesoContadorRequest
{
    [JsonIgnore]
    public Guid AnfitrionId { get; set; }

    [Required]
    public string Justificacion { get; set; } = string.Empty;
}
