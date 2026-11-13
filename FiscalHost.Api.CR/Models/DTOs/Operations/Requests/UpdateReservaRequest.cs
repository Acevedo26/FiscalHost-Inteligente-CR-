using System;
using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Requests;

public class UpdateReservaRequest
{
    private string _razonModificacion = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero.")]
    public decimal Monto { get; set; }

    public DateTime? FechaReserva { get; set; }

    public string? Huesped { get; set; }

    public string? RazonCambio { get; set; }

    public string? Justificacion { get; set; }

    public string RazonModificacion
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_razonModificacion)) return _razonModificacion;
            if (!string.IsNullOrWhiteSpace(RazonCambio)) return RazonCambio;
            if (!string.IsNullOrWhiteSpace(Justificacion)) return Justificacion;
            return string.Empty;
        }
        set => _razonModificacion = value;
    }

    public string ObtenerRazon() => RazonModificacion;
}
