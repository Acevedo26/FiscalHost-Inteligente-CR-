using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.Entities.Operations;

[Table("gasto", Schema = "fiscalhost_db")]
public class Gasto
{
    [Key]
    [Column("gasto_id")]
    public Guid GastoId { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [Column("propiedad_id")]
    public Guid? PropiedadId { get; set; }
    public Propiedad? Propiedad { get; set; }

    [Column("proveedor")]
    [MaxLength(200)]
    public string Proveedor { get; set; } = string.Empty;

    [Column("numero_factura")]
    public string? NumeroFactura { get; set; }

    [Column("clave_numerica_hacienda")]
    public string? ClaveNumericaHacienda { get; set; }

    [Column("fecha_emision")]
    public DateTime FechaEmision { get; set; }

    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [Column("monto_total")]
    public decimal MontoTotal { get; set; }

    [Column("monto_iva_soportado")]
    public decimal MontoIvaSoportado { get; set; }

    [Column("moneda")]
    public TipoMoneda Moneda { get; set; }

    [Column("tipo_cambio")]
    public decimal TipoCambio { get; set; }

    [Column("monto_colones")]
    public decimal MontoColones { get; set; }

    [Column("tipo_gasto")]
    public string TipoGasto { get; set; } = string.Empty;

    [Column("es_deducible_renta")]
    public bool EsDeducibleRenta { get; set; }

    [Column("es_credito_fiscal_valido")]
    public bool EsCreditoFiscalValido { get; set; }

    [Column("evidencia_url")]
    public string? EvidenciaUrl { get; set; }

    [Column("evidencia_nombre_archivo")]
    public string? EvidenciaNombreArchivo { get; set; }

    [Column("estado_ocr")]
    public EstadoOcr? EstadoOcr { get; set; }

    [Column("datos_extraidos_ocr", TypeName = "jsonb")]
    public string DatosExtraidosOcr { get; set; } = "{}";

    [Column("hash_unico_comprobante")]
    public string? HashUnicoComprobante { get; set; }

    [Column("estado_validacion")]
    public EstadoValidacion EstadoValidacion { get; set; }

    [Column("observaciones_validacion")]
    public string? ObservacionesValidacion { get; set; }

    [Column("periodo_fiscal_anio")]
    public short PeriodoFiscalAnio { get; set; }

    [Column("periodo_fiscal_mes")]
    public short PeriodoFiscalMes { get; set; }

    [Column("fuente_registro")]
    public FuenteRegistro FuenteRegistro { get; set; }
}
