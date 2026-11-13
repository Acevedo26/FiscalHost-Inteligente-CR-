using System.Text.Json.Serialization;
using System;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

public class GenerarBorradorD125Request
{
    [JsonIgnore]
    public Guid UsuarioId { get; set; }
    public short Anio { get; set; }
    public bool RegimenUtilidades { get; set; }
}
