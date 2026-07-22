using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.Entities.TaxIntelligence;

// ========================================================================
// Entidad: Representa una tabla en la base de datos PostgreSQL, la cual se
// utiliza exclusivamente por Entity Framework para operaciones de lectura
// y escritura.
// ========================================================================


[Table("periodo_fiscal", Schema = "fiscalhost_db")]
public class PeriodoFiscal
{
	[Key]
	[Column("periodo_id")]
	public Guid PeriodoId { get; set; }

	[Column("anio")]
	public short Anio { get; set; }

	[Column("mes")]
	public short Mes { get; set; }

	[Column("tipo_formulario")]
	public TipoFormulario TipoFormulario { get; set; }

	[Column("fecha_inicio_periodo")]
	public DateTime FechaInicioPeriodo { get; set; }

	[Column("fecha_fin_periodo")]
	public DateTime FechaFinPeriodo { get; set; }

	[Column("fecha_vencimiento")]
	public DateTime FechaVencimiento { get; set; }

	[Column("tarifa_iva")]
	public decimal TarifaIva { get; set; }

	[Column("tarifa_renta_capital")]
	public decimal TarifaRentaCapital { get; set; }

	[Column("deduccion_plana_capital")]
	public decimal DeduccionPlanaCapital { get; set; }

	[Column("tasa_interes_mora_anual")]
	public decimal? TasaInteresMoraAnual { get; set; }

	// HU-011: valor vigente del "salario base" (unidad legal costarricense,
	// Ley 7337) usado para calcular la multa del Art. 79 CNPT por
	// inscripcion tardia (0.5 x salario base). Se guarda por periodo/tipo
	// de formulario D176 porque el salario base cambia anualmente.
	[Column("salario_base_vigente")]
	public decimal? SalarioBaseVigente { get; set; }

	[Column("normativa_aplicable")]
	public string NormativaAplicable { get; set; } = string.Empty;

	public ICollection<CalculoFiscal> CalculosFiscales { get; set; } = new List<CalculoFiscal>();
	public ICollection<ObligacionTributaria> Obligaciones { get; set; } = new List<ObligacionTributaria>();
}
