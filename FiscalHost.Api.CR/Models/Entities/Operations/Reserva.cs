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


[Table("reserva", Schema = "fiscalhost_db")]
public class Reserva
{
    [Key]
    [Column("reserva_id")]
    public Guid ReservaId { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [Column("propiedad_id")]
    public Guid? PropiedadId { get; set; }
    public Propiedad? Propiedad { get; set; }

    [Column("importacion_id")]
    public Guid? ImportacionId { get; set; }
    public ImportacionMasiva? Importacion { get; set; }

    [Column("fecha_inicio")]
    public DateTime FechaInicio { get; set; }

    [Column("fecha_fin")]
    public DateTime FechaFin { get; set; }

    [Column("nombre_huesped")]
    public string? NombreHuesped { get; set; }

    [Column("identificacion_huesped")]
    public string? IdentificacionHuesped { get; set; }

    [Column("pais_origen_huesped")]
    public string? PaisOrigenHuesped { get; set; }

    [Column("monto_bruto")]
    public decimal MontoBruto { get; set; }

    [Column("moneda")]
    public TipoMoneda Moneda { get; set; }

    [Column("tipo_cambio")]
    public decimal TipoCambio { get; set; }

    [Column("monto_colones")]
    public decimal MontoColones { get; set; }

    [Column("clasificacion_fiscal")]
    public ClasificacionFiscal ClasificacionFiscal { get; set; }

    [Column("monto_gravado")]
    public decimal MontoGravado { get; set; }

    [Column("monto_exento")]
    public decimal MontoExento { get; set; }

    [Column("monto_iva_calculado")]
    public decimal MontoIvaCalculado { get; set; }

    [Column("retencion_extranjera")]
    public decimal RetencionExtranjera { get; set; }

    [Column("plataforma_origen")]
    public PlataformaOrigen PlataformaOrigen { get; set; }

    [Column("fuente_registro")]
    public FuenteRegistro FuenteRegistro { get; set; }

    [Column("referencia_plataforma")]
    public string? ReferenciaPlataforma { get; set; }

    [Column("fue_reclasificada")]
    public bool FueReclasificada { get; set; }

    [Column("justificacion_reclasificacion")]
    public string? JustificacionReclasificacion { get; set; }

    [Column("fecha_reclasificacion")]
    public DateTimeOffset? FechaReclasificacion { get; set; }

    [Column("usuario_reclasificacion_id")]
    public Guid? UsuarioReclasificacionId { get; set; }

    [Column("periodo_fiscal_anio")]
    public short PeriodoFiscalAnio { get; set; }

    [Column("periodo_fiscal_mes")]
    public short PeriodoFiscalMes { get; set; }

    [Column("estado")]
    public string Estado { get; set; } = string.Empty;

    [Column("metadata", TypeName = "jsonb")]
    public string Metadata { get; set; } = "{}";
}


