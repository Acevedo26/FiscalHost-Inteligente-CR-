using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;

/// <summary>
/// Representa una fila del CSV de reconstrucción histórica.
/// Formato: Anio,Mes,TipoFormulario,IngresosBrutos,IngresosGravados,IngresosExentos,GastosDeducibles,Retenciones,CreditoFiscal
/// </summary>
public class ReconstruccionHistoricaCsvRow
{
    [Required]
    [Range(2020, 2026)]
    [Name("Anio")]
    public short Anio { get; set; }

    [Required]
    [Range(1, 12)]
    [Name("Mes")]
    public short Mes { get; set; }

    [Required]
    [Name("TipoFormulario")]
    public string TipoFormulario { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue)]
    [Name("IngresosBrutos")]
    public decimal IngresosBrutos { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [Name("IngresosGravados")]
    public decimal IngresosGravados { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [Name("IngresosExentos")]
    public decimal IngresosExentos { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [Name("GastosDeducibles")]
    public decimal GastosDeducibles { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [Name("Retenciones")]
    public decimal Retenciones { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [Name("CreditoFiscal")]
    public decimal CreditoFiscal { get; set; }
}