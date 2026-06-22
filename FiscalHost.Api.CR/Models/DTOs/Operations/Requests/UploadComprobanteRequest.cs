using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Requests;

// ========================================================================
// DTO para la subida de comprobantes con OCR.
// ========================================================================
public class UploadComprobanteRequest
{
    [Required]
    public Guid UsuarioId { get; set; }

    public Guid? PropiedadId { get; set; }

    // El archivo adjunto (PDF o Imagen).
    [Required]
    public IFormFile Comprobante { get; set; } = null!;
}
