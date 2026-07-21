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
    public DbSet<ConfiguracionTributaria> ConfiguracionesTributarias => Set<ConfiguracionTributaria>();

    // DbSets for restored modules
    public DbSet<AuditoriaConfiguracion> AuditoriasConfiguracion => Set<AuditoriaConfiguracion>();
    public DbSet<AuditoriaLlave> AuditoriasLlave => Set<AuditoriaLlave>();
    public DbSet<ReservaDirecta> ReservasDirectas => Set<ReservaDirecta>();
    public DbSet<GastoOperativo> GastosOperativos => Set<GastoOperativo>();
    public DbSet<AuditoriaOperacion> AuditoriasOperacion => Set<AuditoriaOperacion>();
    public DbSet<ClasificacionIngreso> ClasificacionesIngresos => Set<ClasificacionIngreso>();
    public DbSet<AuditoriaClasificacionIngreso> AuditoriasClasificacionIngresos => Set<AuditoriaClasificacionIngreso>();

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
        modelBuilder.HasPostgresEnum<ClasificacionIva>("fiscalhost_db", "clasificacion_iva");
        modelBuilder.HasPostgresEnum<FuenteIngreso>("fiscalhost_db", "fuente_ingreso");

        // ── Mapeo explícito de nombres de tabla (snake_case en PostgreSQL) ──────────────

        // Identity
        modelBuilder.Entity<Usuario>().ToTable("usuario");
        modelBuilder.Entity<PerfilTributario>().ToTable("perfil_tributario");
        modelBuilder.Entity<Propiedad>().ToTable("propiedad");
        modelBuilder.Entity<AccesoContador>().ToTable("acceso_contador");

        // Operations
        modelBuilder.Entity<Reserva>().ToTable("reserva");
        modelBuilder.Entity<Gasto>().ToTable("gasto");
        modelBuilder.Entity<Gasto>()
            .Property(g => g.MontoNeto)
            .ValueGeneratedOnAddOrUpdate();
        modelBuilder.Entity<ImportacionMasiva>().ToTable("importacion_masiva");
        modelBuilder.Entity<ReservaDirecta>().ToTable("reserva_directa");
        modelBuilder.Entity<GastoOperativo>().ToTable("gasto_operativo");
        modelBuilder.Entity<ClasificacionIngreso>().ToTable("clasificacion_ingreso");

        // TaxIntelligence
        modelBuilder.Entity<CatalogoActividadEconomica>().ToTable("catalogo_actividad_economica");
        modelBuilder.Entity<CalculoFiscal>().ToTable("calculo_fiscal");
        modelBuilder.Entity<ObligacionTributaria>().ToTable("obligacion_tributaria");
        modelBuilder.Entity<SancionAutoliquidacion>().ToTable("sancion_autoliquidacion");
        modelBuilder.Entity<PeriodoFiscal>().ToTable("periodo_fiscal");
        modelBuilder.Entity<SimulacionFiscal>().ToTable("simulacion_fiscal");
        modelBuilder.Entity<Exportacion>().ToTable("exportacion");
        modelBuilder.Entity<ConfiguracionTributaria>().ToTable("configuracion_tributaria");

        // Communication
        modelBuilder.Entity<Alerta>().ToTable("alerta");
        modelBuilder.Entity<ContenidoEducativo>().ToTable("contenido_educativo");

        // Audit
        modelBuilder.Entity<LlaveCriptografica>().ToTable("llave_criptografica");
        modelBuilder.Entity<AuditoriaConfiguracion>().ToTable("auditoria_configuracion");
        modelBuilder.Entity<AuditoriaLlave>().ToTable("auditoria_llave");
        modelBuilder.Entity<AuditoriaOperacion>().ToTable("audit_log");
        modelBuilder.Entity<AuditoriaOperacion>(e =>
        {
            e.HasKey(a => a.AuditId);
            e.Property(a => a.AuditId).HasColumnName("audit_id");
            e.Property(a => a.UsuarioId).HasColumnName("usuario_id");
            e.Property(a => a.CorreoUsuario).HasColumnName("correo_usuario");
            e.Property(a => a.RolUsuario).HasColumnName("rol_usuario");
            e.Property(a => a.Operacion).HasColumnName("operacion");
            e.Property(a => a.TablaAfectada).HasColumnName("tabla_afectada");
            e.Property(a => a.RegistroId).HasColumnName("registro_id");
            e.Property(a => a.OldValues).HasColumnName("old_values").HasColumnType("jsonb");
            e.Property(a => a.NewValues).HasColumnName("new_values").HasColumnType("jsonb");
            e.Property(a => a.CamposModificados).HasColumnName("campos_modificados");
            e.Property(a => a.Justificacion).HasColumnName("justificacion");
            e.Property(a => a.IpOrigen).HasColumnName("ip_origen").HasColumnType("inet");
            e.Property(a => a.UserAgent).HasColumnName("user_agent");
            e.Property(a => a.RequestId).HasColumnName("request_id");
            e.Property(a => a.CreatedAt).HasColumnName("created_at");
        });
        modelBuilder.Entity<AuditoriaClasificacionIngreso>().ToTable("auditoria_clasificacion_ingreso");

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

        modelBuilder.Entity<ClasificacionIngreso>(e =>
        {
            e.Property(c => c.AnfitrionId).HasMaxLength(50).IsRequired();
            e.Property(c => c.MontoBruto).HasPrecision(18, 2);
            e.Property(c => c.MontoIva).HasPrecision(18, 2);
            e.Property(c => c.BaseImponibleRenta).HasPrecision(18, 2);
            e.Property(c => c.ImpuestoRenta).HasPrecision(18, 2);
            e.Property(c => c.MontoRetencion).HasPrecision(18, 2);
            e.Property(c => c.NetoAnfitrion).HasPrecision(18, 2);
            e.Property(c => c.JustificacionManual).HasMaxLength(500);
            e.HasIndex(c => c.AnfitrionId);
        });

        modelBuilder.Entity<AuditoriaClasificacionIngreso>(e =>
        {
            e.Property(a => a.UsuarioId).HasMaxLength(50).IsRequired();
            e.Property(a => a.ValorAnterior).HasMaxLength(100).IsRequired();
            e.Property(a => a.ValorNuevo).HasMaxLength(100).IsRequired();
            e.Property(a => a.Justificacion).HasMaxLength(500).IsRequired();
            e.HasOne(a => a.ClasificacionIngreso)
             .WithMany(c => c.Auditorias)
             .HasForeignKey(a => a.ClasificacionIngresoId);
        });

        modelBuilder.HasPostgresEnum<EstadoLlave>("estado_llave", "fiscalhost_db");

        // Opcional: configuración explícita por entidad
        modelBuilder.Entity<LlaveCriptografica>()
            .Property(e => e.Estado)
            .HasColumnType("fiscalhost_db.estado_llave");
}
}
