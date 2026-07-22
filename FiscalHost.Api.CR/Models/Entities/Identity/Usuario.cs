using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Models.Entities.Identity;

// ========================================================================
// Entidad: Representa una tabla en la base de datos PostgreSQL, la cual se
// utiliza exclusivamente por Entity Framework para operaciones de lectura
// y escritura.
// ========================================================================


[Table("usuario", Schema = "fiscalhost_db")]
public class Usuario
{
    [Key]
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    [Column("tipo_identificacion")]
    public TipoIdentificacion TipoIdentificacion { get; set; }

    [Column("numero_identificacion")]
    [MaxLength(50)]
    public string NumeroIdentificacion { get; set; } = string.Empty;

    [Column("nombre_completo")]
    [MaxLength(200)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Column("razon_social")]
    [MaxLength(200)]
    public string? RazonSocial { get; set; }

    [Column("correo_electronico")]
    [MaxLength(150)]
    public string CorreoElectronico { get; set; } = string.Empty;

    [Column("contrasena_hash")]
    [MaxLength(255)]
    public string ContrasenaHash { get; set; } = string.Empty;

    [Column("estado")]
    public EstadoUsuario Estado { get; set; }

    [Column("rol_principal")]
    public RolUsuario RolPrincipal { get; set; }

    [Column("es_usuario_nuevo")]
    public bool EsUsuarioNuevo { get; set; }

    [Column("correo_verificado")]
    public bool CorreoVerificado { get; set; }

    [Column("preferencias_notificacion", TypeName = "jsonb")]
    public string PreferenciasNotificacion { get; set; } = "{}";

    [Column("fecha_activacion")]
    public DateTimeOffset? FechaActivacion { get; set; }

    [Column("ultimo_acceso")]
    public DateTimeOffset? UltimoAcceso { get; set; }

    // Relaciones (Propiedades de Navegación)
    public PerfilTributario? PerfilTributario { get; set; }
    
    public ICollection<Propiedad> Propiedades { get; set; } = new List<Propiedad>();
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    public ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();
    public ICollection<CalculoFiscal> CalculosFiscales { get; set; } = new List<CalculoFiscal>();
    public ICollection<ObligacionTributaria> Obligaciones { get; set; } = new List<ObligacionTributaria>();
    public ICollection<ImportacionMasiva> Importaciones { get; set; } = new List<ImportacionMasiva>();
    public ICollection<Exportacion> Exportaciones { get; set; } = new List<Exportacion>();
    public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
    public ICollection<LlaveCriptografica> LlavesCriptograficas { get; set; } = new List<LlaveCriptografica>();
    public ICollection<SimulacionFiscal> SimulacionesFiscales { get; set; } = new List<SimulacionFiscal>();
    public ICollection<ContenidoEducativo> ContenidosEducativos { get; set; } = new List<ContenidoEducativo>();
    
    [InverseProperty(nameof(AccesoContador.Anfitrion))]
    public ICollection<AccesoContador> AccesosAnfitrion { get; set; } = new List<AccesoContador>();

    [InverseProperty(nameof(AccesoContador.Contador))]
    public ICollection<AccesoContador> AccesosContador { get; set; } = new List<AccesoContador>();
}


