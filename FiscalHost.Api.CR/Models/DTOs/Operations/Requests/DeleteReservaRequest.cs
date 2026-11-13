using System;
using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Requests;

public class DeleteReservaRequest
{
    private string _razonEliminacion = string.Empty;

    public string? RazonCambio { get; set; }

    public string? Justificacion { get; set; }

    public string RazonEliminacion
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_razonEliminacion)) return _razonEliminacion;
            if (!string.IsNullOrWhiteSpace(RazonCambio)) return RazonCambio;
            if (!string.IsNullOrWhiteSpace(Justificacion)) return Justificacion;
            return string.Empty;
        }
        set => _razonEliminacion = value;
    }

    public string ObtenerRazon() => RazonEliminacion;
}
