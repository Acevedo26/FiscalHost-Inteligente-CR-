using System;
using System.ComponentModel.DataAnnotations;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Requests;

// ========================================================================
// DTO de Actualización de Gasto: Se utiliza para modificar un gasto.
// Se exige la justificación de manera obligatoria para cumplir con la Ley 8968.
// ========================================================================
public class UpdateGastoRequest
{
    // Campo de auditoría estricto según la ley
    [Required(ErrorMessage = "La justificación es obligatoria según la Ley 8968.")]
    [MinLength(10, ErrorMessage = "La justificación debe detallar el motivo del cambio.")]
    public string Justificacion { get; set; } = string.Empty;

    // Campos que se pueden modificar.
    [Required]
    [MaxLength(200)]
    public string Proveedor { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? NumeroFactura { get; set; }

    [Required]
    public DateOnly FechaEmision { get; set; }

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

    public string? Descripcion { get; set; }
}
