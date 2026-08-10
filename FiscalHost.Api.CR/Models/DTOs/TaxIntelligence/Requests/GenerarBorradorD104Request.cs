using System;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

public class GenerarBorradorD104Request
{
    public Guid UsuarioId { get; set; }
    public short Anio { get; set; }
    public short Mes { get; set; }
}
