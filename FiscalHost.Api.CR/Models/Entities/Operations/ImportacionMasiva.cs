using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.Entities.Operations;

[Table("importacion_masiva", Schema = "fiscalhost_db")]
public class ImportacionMasiva
{
    [Key]
    [Column("importacion_id")]
    public Guid ImportacionId { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [Column("tipo_importacion")]
    public string TipoImportacion { get; set; } = string.Empty;

    [Column("plataforma_origen")]
    public PlataformaOrigen? PlataformaOrigen { get; set; }

    [Column("archivo_url")]
    public string ArchivoUrl { get; set; } = string.Empty;

    [Column("nombre_archivo_original")]
    public string NombreArchivoOriginal { get; set; } = string.Empty;

    [Column("plantilla_utilizada")]
    public string? PlantillaUtilizada { get; set; }

    [Column("tamanio_bytes")]
    public long? TamanioBytes { get; set; }

    [Column("estado")]
    public EstadoImportacion Estado { get; set; }

    [Column("total_registros")]
    public int TotalRegistros { get; set; }

    [Column("registros_exitosos")]
    public int RegistrosExitosos { get; set; }

    [Column("registros_con_error")]
    public int RegistrosConError { get; set; }

    [Column("reporte_errores_url")]
    public string? ReporteErroresUrl { get; set; }

    [Column("detalle_errores", TypeName = "jsonb")]
    public string DetalleErrores { get; set; } = "{}";

    [Column("fecha_inicio_procesamiento")]
    public DateTimeOffset? FechaInicioProcesamiento { get; set; }

    [Column("fecha_fin_procesamiento")]
    public DateTimeOffset? FechaFinProcesamiento { get; set; }

    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
