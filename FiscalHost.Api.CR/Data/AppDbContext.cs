using FiscalHost.Api.CR.Models.Enums;
using FiscalHost.Api.CR.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<PerfilTributario> PerfilesTributarios => Set<PerfilTributario>();
    public DbSet<CatalogoActividadEconomica> CatalogoActividadesEconomicas => Set<CatalogoActividadEconomica>();
    public DbSet<Propiedad> Propiedades => Set<Propiedad>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Gasto> Gastos => Set<Gasto>();
    public DbSet<ImportacionMasiva> ImportacionesMasivas => Set<ImportacionMasiva>();
    public DbSet<CalculoFiscal> CalculosFiscales => Set<CalculoFiscal>();
    public DbSet<ObligacionTributaria> ObligacionesTributarias => Set<ObligacionTributaria>();
    public DbSet<SancionAutoliquidacion> SancionesAutoliquidacion => Set<SancionAutoliquidacion>();
    public DbSet<Alerta> Alertas => Set<Alerta>();
    public DbSet<Exportacion> Exportaciones => Set<Exportacion>();
    public DbSet<LlaveCriptografica> LlavesCriptograficas => Set<LlaveCriptografica>();
    public DbSet<SimulacionFiscal> SimulacionesFiscales => Set<SimulacionFiscal>();
    public DbSet<ContenidoEducativo> ContenidosEducativos => Set<ContenidoEducativo>();
    public DbSet<AccesoContador> AccesosContadores => Set<AccesoContador>();
    public DbSet<PeriodoFiscal> PeriodosFiscales => Set<PeriodoFiscal>();

    // DbSets for restored modules
    public DbSet<AuditoriaConfiguracion> AuditoriasConfiguracion => Set<AuditoriaConfiguracion>();
    public DbSet<AuditoriaLlave> AuditoriasLlave => Set<AuditoriaLlave>();
    public DbSet<ReservaDirecta> ReservasDirectas => Set<ReservaDirecta>();
    public DbSet<GastoOperativo> GastosOperativos => Set<GastoOperativo>();
    public DbSet<AuditoriaOperacion> AuditoriasOperacion => Set<AuditoriaOperacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("fiscalhost_db");

        modelBuilder.HasPostgresEnum<TipoIdentificacion>("fiscalhost_db", "tipo_identificacion");
        modelBuilder.HasPostgresEnum<EstadoUsuario>("fiscalhost_db", "estado_usuario");
        modelBuilder.HasPostgresEnum<RolUsuario>("fiscalhost_db", "rol_usuario");
        modelBuilder.HasPostgresEnum<RegimenTributario>("fiscalhost_db", "regimen_tributario");
        modelBuilder.HasPostgresEnum<ClasificacionFiscal>("fiscalhost_db", "clasificacion_fiscal");
        modelBuilder.HasPostgresEnum<PlataformaOrigen>("fiscalhost_db", "plataforma_origen");
        modelBuilder.HasPostgresEnum<FuenteRegistro>("fiscalhost_db", "fuente_registro");
        modelBuilder.HasPostgresEnum<EstadoOcr>("fiscalhost_db", "estado_ocr");
        modelBuilder.HasPostgresEnum<EstadoValidacion>("fiscalhost_db", "estado_validacion");
        modelBuilder.HasPostgresEnum<EstadoImportacion>("fiscalhost_db", "estado_importacion");
        modelBuilder.HasPostgresEnum<TipoFormulario>("fiscalhost_db", "tipo_formulario");
        modelBuilder.HasPostgresEnum<EstadoDeclaracion>("fiscalhost_db", "estado_declaracion");
        modelBuilder.HasPostgresEnum<EstadoObligacion>("fiscalhost_db", "estado_obligacion");
        modelBuilder.HasPostgresEnum<TipoAlerta>("fiscalhost_db", "tipo_alerta");
        modelBuilder.HasPostgresEnum<CanalNotificacion>("fiscalhost_db", "canal_notificacion");
        modelBuilder.HasPostgresEnum<EstadoNotificacion>("fiscalhost_db", "estado_notificacion");
        modelBuilder.HasPostgresEnum<EstadoLlave>("fiscalhost_db", "estado_llave");
        modelBuilder.HasPostgresEnum<OperacionAuditoria>("fiscalhost_db", "operacion_auditoria");
        modelBuilder.HasPostgresEnum<TipoMoneda>("fiscalhost_db", "tipo_moneda");

        // ── Mapeo explícito de nombres de tabla (snake_case en PostgreSQL) ──────────────

        // Identity
        modelBuilder.Entity<Usuario>().ToTable("usuario");
        modelBuilder.Entity<PerfilTributario>().ToTable("perfil_tributario");
        modelBuilder.Entity<Propiedad>().ToTable("propiedad");
        modelBuilder.Entity<AccesoContador>().ToTable("acceso_contador");

        // Operations
        modelBuilder.Entity<Reserva>().ToTable("reserva");
        modelBuilder.Entity<Gasto>().ToTable("gasto");
        modelBuilder.Entity<ImportacionMasiva>().ToTable("importacion_masiva");
        modelBuilder.Entity<ReservaDirecta>().ToTable("reserva_directa");
        modelBuilder.Entity<GastoOperativo>().ToTable("gasto_operativo");

        // TaxIntelligence
        modelBuilder.Entity<CatalogoActividadEconomica>().ToTable("catalogo_actividad_economica");
        modelBuilder.Entity<CalculoFiscal>().ToTable("calculo_fiscal");
        modelBuilder.Entity<ObligacionTributaria>().ToTable("obligacion_tributaria");
        modelBuilder.Entity<SancionAutoliquidacion>().ToTable("sancion_autoliquidacion");
        modelBuilder.Entity<PeriodoFiscal>().ToTable("periodo_fiscal");
        modelBuilder.Entity<SimulacionFiscal>().ToTable("simulacion_fiscal");
        modelBuilder.Entity<Exportacion>().ToTable("exportacion");

        // Communication
        modelBuilder.Entity<Alerta>().ToTable("alerta");
        modelBuilder.Entity<ContenidoEducativo>().ToTable("contenido_educativo");

        // Audit
        modelBuilder.Entity<LlaveCriptografica>().ToTable("llave_criptografica");
        modelBuilder.Entity<AuditoriaConfiguracion>().ToTable("auditoria_configuracion");
        modelBuilder.Entity<AuditoriaLlave>().ToTable("auditoria_llave");
        modelBuilder.Entity<AuditoriaOperacion>().ToTable("auditoria_operacion");

        // Índice único en catálogo de actividades
        modelBuilder.Entity<CatalogoActividadEconomica>(e =>
        {
            e.HasIndex(a => a.Codigo).IsUnique();
        });

        // ── Configuración explícita para AccesoContador (evitar cascadas múltiples) ───
        modelBuilder.Entity<AccesoContador>()
            .HasOne(a => a.Anfitrion)
            .WithMany(u => u.AccesosAnfitrion)
            .HasForeignKey(a => a.AnfitrionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AccesoContador>()
            .HasOne(a => a.Contador)
            .WithMany(u => u.AccesosContador)
            .HasForeignKey(a => a.ContadorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
