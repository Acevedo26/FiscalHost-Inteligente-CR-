using System;
using System.ComponentModel.DataAnnotations;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Requests;

// ========================================================================
// DTO de Request (Entrada): Contiene, únicamente, los datos que el usuario
// o frontend envía a la API al realizar una petición, sin incluir
// identificadores generados ni campos de auditoría internos.
// ========================================================================


public class CreateImportacionMasivaRequest
{
    [Required]
    public string TipoImportacion { get; set; } = string.Empty;

    public PlataformaOrigen? PlataformaOrigen { get; set; }

    public string? PlantillaUtilizada { get; set; }
    
    // ArchivoUrl and NombreArchivoOriginal should be derived from the uploaded file in the controller
}


