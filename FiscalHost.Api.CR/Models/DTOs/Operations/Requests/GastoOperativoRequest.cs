using System;
using System.ComponentModel.DataAnnotations;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Requests;

// ========================================================================
// DTO de Request (Entrada): Contiene, únicamente, los datos que el usuario
// o frontend envía a la API al realizar una petición, sin incluir
// identificadores generados ni campos de auditoría internos.
// ========================================================================


public class GastoOperativoRequest
{
    [Required]
    public Guid UsuarioId { get; set; }

    public Guid? PropiedadId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Proveedor { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? NumeroFactura { get; set; }

    [MaxLength(50)]
    public string? ClaveNumericaHacienda { get; set; }

    [Required]
    public DateOnly FechaEmision { get; set; }

    public string? Descripcion { get; set; }

    [Required]
    public decimal MontoTotal { get; set; }

    [Required]
    public decimal MontoIvaSoportado { get; set; }

    public decimal? MontoNeto { get; set; }

    [Required]
    public TipoMoneda Moneda { get; set; }

    [Required]
    [MaxLength(50)]
    public string TipoGasto { get; set; } = string.Empty;

    [Required]
    public bool EsDeducibleRenta { get; set; }

    [Required]
    public bool EsCreditoFiscalValido { get; set; }

    [MaxLength(500)]
    public string? EvidenciaUrl { get; set; }

    [MaxLength(255)]
    public string? EvidenciaNombreArchivo { get; set; }

    [MaxLength(50)]
    public string? EvidenciaTipoMime { get; set; }

    public long? EvidenciaTamanioBytes { get; set; }
}


