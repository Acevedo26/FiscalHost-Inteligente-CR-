using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

/// <summary>
/// Request para la reconstrucción histórica retroactiva mediante carga de CSV.
/// </summary>
public class ReconstruccionHistoricaRequest
{
    [Required]
    public IFormFile Archivo { get; set; } = null!;

    [JsonIgnore]
    public Guid UsuarioId { get; set; }
}