using System;
using System.ComponentModel.DataAnnotations;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Requests;

public class CreatePropiedadRequest
{
    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    public string? Direccion { get; set; }

    public string? Provincia { get; set; }

    public string? Canton { get; set; }

    public string? Distrito { get; set; }

    public string? NumeroFinca { get; set; }

    public decimal? ValorFiscal { get; set; }

    public TipoMoneda? TipoMonedaValor { get; set; }
}
