using System;
using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

// ========================================================================
// DTO de Request (Entrada): Contiene, únicamente, los datos que el usuario
// o frontend envía a la API al realizar una petición, sin incluir
// identificadores generados ni campos de auditoría internos.
// ========================================================================


public class CreateSimulacionFiscalRequest
{
    [Required]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    [Required]
    public short PeriodoBaseAnio { get; set; }

    public short? PeriodoBaseMes { get; set; }

    [Required]
    public string ParametrosEntrada { get; set; } = "{}";
}


