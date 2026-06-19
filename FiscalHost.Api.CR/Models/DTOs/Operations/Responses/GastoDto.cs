using System;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.DTOs.Operations.Responses;

public class GastoDto
{
    public Guid GastoId { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid? PropiedadId { get; set; }
    public string Proveedor { get; set; } = string.Empty;
    public string? NumeroFactura { get; set; }
    public string? ClaveNumericaHacienda { get; set; }
    public DateTime FechaEmision { get; set; }
    public string? Descripcion { get; set; }
    public decimal MontoTotal { get; set; }
    public decimal MontoIvaSoportado { get; set; }
    public TipoMoneda Moneda { get; set; }
    public decimal TipoCambio { get; set; }
    public decimal MontoColones { get; set; }
    public string TipoGasto { get; set; } = string.Empty;
    public bool EsDeducibleRenta { get; set; }
    public bool EsCreditoFiscalValido { get; set; }
    public string? EvidenciaUrl { get; set; }
    public string? EvidenciaNombreArchivo { get; set; }
    public EstadoOcr? EstadoOcr { get; set; }
    public string DatosExtraidosOcr { get; set; } = "{}";
    public string? HashUnicoComprobante { get; set; }
    public EstadoValidacion EstadoValidacion { get; set; }
    public string? ObservacionesValidacion { get; set; }
    public short PeriodoFiscalAnio { get; set; }
    public short PeriodoFiscalMes { get; set; }
    public FuenteRegistro FuenteRegistro { get; set; }
}
