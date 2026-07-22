using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

public class ExportacionHaciendaRequest
{
    [Required]
    public Guid UsuarioId { get; set; }

    public Guid? CalculoId { get; set; }

    [Required]
    public short AnioFiscal { get; set; }

    public short? Mes { get; set; }

    [Required]
    public string Formato { get; set; } = "XML";

    [Required]
    public string TipoContenido { get; set; } = "DECLARACION";

    public bool ProtegerConContrasena { get; set; }

    public string? Contrasena { get; set; }
}
