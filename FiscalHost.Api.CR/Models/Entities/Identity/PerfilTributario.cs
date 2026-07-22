using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FiscalHost.Api.CR.Models.Enums;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;

namespace FiscalHost.Api.CR.Models.Entities.Identity;

// ========================================================================
// Entidad: Representa una tabla en la base de datos PostgreSQL, la cual se
// utiliza exclusivamente por Entity Framework para operaciones de lectura
// y escritura.
// ========================================================================


[Table("perfil_tributario", Schema = "fiscalhost_db")]
public class PerfilTributario
{
    [Key]
    [Column("perfil_id")]
    public Guid PerfilId { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [Column("codigo_actividad_economica")]
    [MaxLength(20)]
    public string CodigoActividadEconomica { get; set; } = string.Empty;

    [ForeignKey("CodigoActividadEconomica")]
    public CatalogoActividadEconomica? ActividadEconomica { get; set; }

    [Column("descripcion_actividad")]
    public string DescripcionActividad { get; set; } = string.Empty;

    [Column("tribu_cr")]
    [MaxLength(50)]
    public string? TribuCr { get; set; }

    [Column("direccion_fiscal")]
    public string? DireccionFiscal { get; set; }

    [Column("nise")]
    [MaxLength(50)]
    public string? Nise { get; set; }

    [Column("es_domicilio_validado")]
    public bool EsDomicilioValidado { get; set; }

    [Column("regimen_tributario")]
    public RegimenTributario RegimenTributario { get; set; }

    [Column("fecha_inicio_actividad")]
    public DateTime? FechaInicioActividad { get; set; }

    [Column("fecha_inscripcion_hacienda")]
    public DateTime? FechaInscripcionHacienda { get; set; }

    [Column("datos_complementarios", TypeName = "jsonb")]
    public string DatosComplementarios { get; set; } = "{}";
}


