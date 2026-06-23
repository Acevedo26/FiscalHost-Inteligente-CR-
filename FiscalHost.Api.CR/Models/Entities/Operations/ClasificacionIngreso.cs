using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiscalHost.Api.CR.Models.Entities.Operations;

[Table("clasificacion_ingreso", Schema = "fiscalhost_db")]
public class ClasificacionIngreso
{
    [Key]
    [Column("clasificacion_ingreso_id")]
    public int Id { get; set; }

    [Column("anfitrion_id")]
    public string AnfitrionId { get; set; } = string.Empty;

    [Column("fecha_entrada")]
    public DateTime FechaEntrada { get; set; }

    [Column("fecha_salida")]
    public DateTime FechaSalida { get; set; }

    [Column("dias_estancia")]
    public int DiasEstancia { get; set; }

    [Column("monto_bruto")]
    public decimal MontoBruto { get; set; }

    [Column("fuente_ingreso")]
    public FuenteIngreso FuenteIngreso { get; set; }

    [Column("tiene_factura_electronica_nacional")]
    public bool TieneFacturaElectronicaNacional { get; set; }

    [Column("huesped_residente")]
    public bool HuespedResidente { get; set; } = true;

    [Column("clasificacion_iva")]
    public ClasificacionIva ClasificacionIva { get; set; }

    [Column("monto_iva")]
    public decimal MontoIva { get; set; }

    [Column("base_imponible_renta")]
    public decimal BaseImponibleRenta { get; set; }

    [Column("impuesto_renta")]
    public decimal ImpuestoRenta { get; set; }

    [Column("monto_retencion")]
    public decimal MontoRetencion { get; set; }

    [Column("neto_anfitrion")]
    public decimal NetoAnfitrion { get; set; }

    [Column("reclasificado_manualmente")]
    public bool ReclasificadoManualmente { get; set; }

    [Column("justificacion_manual")]
    public string? JustificacionManual { get; set; }

    [Column("fecha_creacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    [Column("fecha_actualizacion")]
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

    public ICollection<AuditoriaClasificacionIngreso> Auditorias { get; set; } = [];
}
