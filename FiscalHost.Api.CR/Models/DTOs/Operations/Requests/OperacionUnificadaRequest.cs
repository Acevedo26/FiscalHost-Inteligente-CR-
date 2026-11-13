using System;
using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Requests;

public class OperacionUnificadaRequest
{
    public string Tipo { get; set; } = "INGRESO"; // "INGRESO" o "GASTO"
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero.")]
    public decimal Monto { get; set; }

    public string Concepto { get; set; } = string.Empty;
    public string? Huesped { get; set; }
    public string? Proveedor { get; set; }
    public string? NumeroFactura { get; set; }
    public string? Clasificacion { get; set; }
    public string? Fuente { get; set; }
}
