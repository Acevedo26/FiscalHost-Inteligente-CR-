using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.Entities.Operations;

// ========================================================================
// Entidad: Representa una tabla en la base de datos PostgreSQL, la cual se
// utiliza exclusivamente por Entity Framework para operaciones de lectura
// y escritura.
// ========================================================================


[Table("gasto", Schema = "fiscalhost_db")]
public class Gasto
{
    [Key]
    [Required]
    [Column("gasto_id")]
    public Guid GastoId { get; set; }

    // RelaciÃ³n con el usuario que registra el gasto
    [Required]
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [Column("propiedad_id")]
    public Guid? PropiedadId { get; set; }
    public Propiedad? Propiedad { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("proveedor")]
    public string Proveedor { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("numero_factura")]
    public string? NumeroFactura { get; set; }

    [MaxLength(50)]
    [Column("clave_numerica_hacienda")]
    public string? ClaveNumericaHacienda { get; set; }

    [Required]
    [Column("fecha_emision", TypeName = "date")]
    public DateOnly FechaEmision { get; set; }

    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [Required]
    [Column("monto_total")]
    public decimal MontoTotal { get; set; }

    [Required]
    [Column("monto_iva_soportado")]
    public decimal MontoIvaSoportado { get; set; }

    [Column("monto_neto")]
    public decimal? MontoNeto { get; set; }

    [Required]
    [Column("moneda")]
    public TipoMoneda Moneda { get; set; }

    [Column("tipo_cambio")]
    public decimal? TipoCambio { get; set; }

    [Required]
    [Column("monto_colones")]
    public decimal MontoColones { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("tipo_gasto")]
    public string TipoGasto { get; set; } = string.Empty;

    [Required]
    [Column("es_deducible_renta")]
    public bool EsDeducibleRenta { get; set; }

    [Required]
    [Column("es_credito_fiscal_valido")]
    public bool EsCreditoFiscalValido { get; set; }

    [MaxLength(500)]
    [Column("evidencia_url")]
    public string? EvidenciaUrl { get; set; }

    [MaxLength(255)]
    [Column("evidencia_nombre_archivo")]
    public string? EvidenciaNombreArchivo { get; set; }

    [MaxLength(50)]
    [Column("evidencia_tipo_mime")]
    public string? EvidenciaTipoMime { get; set; }

    [Column("evidencia_tamanio_bytes")]
    public long? EvidenciaTamanioBytes { get; set; }

    [Column("estado_ocr")]
    public EstadoOcr? EstadoOcr { get; set; }

    [Column("datos_extraidos_ocr", TypeName = "jsonb")]
    public string DatosExtraidosOcr { get; set; } = "{}";

    [MaxLength(64)]
    [Column("hash_unico_comprobante")]
    public string? HashUnicoComprobante { get; set; }

    [Required]
    [Column("estado_validacion")]
    public EstadoValidacion EstadoValidacion { get; set; }

    [Column("observaciones_validacion")]
    public string? ObservacionesValidacion { get; set; }

    [Required]
    [Column("periodo_fiscal_anio")]
    public short PeriodoFiscalAnio { get; set; }

    [Required]
    [Column("periodo_fiscal_mes")]
    public short PeriodoFiscalMes { get; set; }

    [Required]
    [Column("fuente_registro")]
    public FuenteRegistro FuenteRegistro { get; set; }

    [Column("fecha_registro")]
    public DateTimeOffset FechaRegistro { get; set; } = DateTimeOffset.UtcNow;

    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    [Column("deleted_at")]
    public DateTimeOffset? DeletedAt { get; set; }
}


