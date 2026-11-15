using System.Text.Json.Serialization;
using System;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

public class GenerarCalculoIvaRequest
{
    public short Anio { get; set; }
    public short Mes { get; set; }
}
