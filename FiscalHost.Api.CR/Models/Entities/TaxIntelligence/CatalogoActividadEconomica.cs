using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiscalHost.Api.CR.Models.Entities.TaxIntelligence;

// ========================================================================
// Entidad: Representa una tabla en la base de datos PostgreSQL, la cual se
// utiliza exclusivamente por Entity Framework para operaciones de lectura
// y escritura.
// ========================================================================


[Table("catalogo_actividad_economica", Schema = "fiscalhost_db")]
public class CatalogoActividadEconomica
{
    [Key]
    [Column("codigo")]
    [MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;

    [Column("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    [Column("seccion")]
    public string? Seccion { get; set; }

    [Column("tarifa_iva")]
    public decimal TarifaIva { get; set; }

    [Column("vigente")]
    public bool Vigente { get; set; }

    [Column("fecha_vigencia_desde")]
    public DateTime FechaVigenciaDesde { get; set; }

    [Column("fecha_vigencia_hasta")]
    public DateTime? FechaVigenciaHasta { get; set; }

    public ICollection<PerfilTributario> PerfilesTributarios { get; set; } = new List<PerfilTributario>();
}


