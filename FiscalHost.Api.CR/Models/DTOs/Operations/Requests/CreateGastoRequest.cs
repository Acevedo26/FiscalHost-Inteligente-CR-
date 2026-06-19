using System;
using System.ComponentModel.DataAnnotations;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Requests;

public class CreateGastoRequest
{
    public Guid? PropiedadId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Proveedor { get; set; } = string.Empty;

    public string? NumeroFactura { get; set; }

    public string? ClaveNumericaHacienda { get; set; }

    [Required]
    public DateTime FechaEmision { get; set; }

    public string? Descripcion { get; set; }

    [Required]
    public decimal MontoTotal { get; set; }

    [Required]
    public decimal MontoIvaSoportado { get; set; }

    [Required]
    public TipoMoneda Moneda { get; set; }

    [Required]
    public decimal TipoCambio { get; set; }

    [Required]
    public string TipoGasto { get; set; } = string.Empty;

    [Required]
    public bool EsDeducibleRenta { get; set; }

    [Required]
    public bool EsCreditoFiscalValido { get; set; }

    [Required]
    public short PeriodoFiscalAnio { get; set; }

    [Required]
    public short PeriodoFiscalMes { get; set; }
}
