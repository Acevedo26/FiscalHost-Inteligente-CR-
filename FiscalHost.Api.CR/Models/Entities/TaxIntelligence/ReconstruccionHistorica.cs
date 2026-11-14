using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.Entities.TaxIntelligence;

/// <summary>
/// Entidad para rastrear procesos de reconstrucción histórica retroactiva.
/// Similar a ImportacionMasiva pero para cálculos fiscales históricos.
/// </summary>
[Table("reconstruccion_historica", Schema = "fiscalhost_db")]
public class ReconstruccionHistorica
{
    [Key]
    [Required]
    [Column("reconstruccion_id")]
    public Guid ReconstruccionId { get; set; }

    [Required]
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    [Column("nombre_archivo_original")]
    public string NombreArchivoOriginal { get; set; } = string.Empty;

    [Column("tamanio_bytes")]
    public long TamanioBytes { get; set; }

    [Required]
    [Column("estado")]
    public EstadoImportacion Estado { get; set; }

    [Column("total_registros")]
    public int TotalRegistros { get; set; }

    [Column("registros_exitosos")]
    public int RegistrosExitosos { get; set; }

    [Column("registros_con_error")]
    public int RegistrosConError { get; set; }

    [Column("detalle_errores", TypeName = "jsonb")]
    public string DetalleErrores { get; set; } = "[]";

    [Column("reporte_errores_url", TypeName = "text")]
    public string? ReporteErroresUrl { get; set; }

    [Column("fecha_inicio_procesamiento")]
    public DateTimeOffset FechaInicioProcesamiento { get; set; } = DateTimeOffset.UtcNow;

    [Column("fecha_fin_procesamiento")]
    public DateTimeOffset? FechaFinProcesamiento { get; set; }
}