using System;
using System.ComponentModel.DataAnnotations;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Requests;

// ========================================================================
// DTO de Request (Entrada): Contiene, únicamente, los datos que el usuario
// o frontend envía a la API al realizar una petición, sin incluir
// identificadores generados ni campos de auditoría internos.
// ========================================================================


public class CreateReservaRequest
{
    public Guid? PropiedadId { get; set; }

    [Required]
    public DateTime FechaInicio { get; set; }

    [Required]
    public DateTime FechaFin { get; set; }

    public string? NombreHuesped { get; set; }

    public string? IdentificacionHuesped { get; set; }

    public string? PaisOrigenHuesped { get; set; }

    [Required]
    public decimal MontoBruto { get; set; }

    [Required]
    public TipoMoneda Moneda { get; set; }

    [Required]
    public decimal TipoCambio { get; set; }

    [Required]
    public ClasificacionFiscal ClasificacionFiscal { get; set; }

    [Required]
    public PlataformaOrigen PlataformaOrigen { get; set; }

    public string? ReferenciaPlataforma { get; set; }

    [Required]
    public short PeriodoFiscalAnio { get; set; }

    [Required]
    public short PeriodoFiscalMes { get; set; }

    public string Metadata { get; set; } = "{}";
}


