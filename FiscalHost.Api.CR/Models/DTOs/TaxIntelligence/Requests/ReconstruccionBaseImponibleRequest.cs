namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

public class ReconstruccionBaseImponibleRequest
{
    public Guid UsuarioId { get; set; }
    public short AnioFiscal { get; set; }
    public bool ContinuarConDatosIncompletos { get; set; }
}
