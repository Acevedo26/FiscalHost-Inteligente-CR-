using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.Entities.Identity;

[Table("propiedad", Schema = "fiscalhost_db")]
public class Propiedad
{
    [Key]
    [Column("propiedad_id")]
    public Guid PropiedadId { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [Column("nombre")]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [Column("direccion")]
    public string? Direccion { get; set; }

    [Column("provincia")]
    public string? Provincia { get; set; }

    [Column("canton")]
    public string? Canton { get; set; }

    [Column("distrito")]
    public string? Distrito { get; set; }

    [Column("numero_finca")]
    public string? NumeroFinca { get; set; }

    [Column("valor_fiscal")]
    public decimal? ValorFiscal { get; set; }

    [Column("tipo_moneda_valor")]
    public TipoMoneda? TipoMonedaValor { get; set; }

    [Column("activa")]
    public bool Activa { get; set; }

    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    public ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();
}
