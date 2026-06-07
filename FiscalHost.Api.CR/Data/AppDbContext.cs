using FiscalHost.Api.CR.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ActividadEconomica> ActividadesEconomicas => Set<ActividadEconomica>();
    public DbSet<ConfiguracionTributaria> ConfiguracionesTributarias => Set<ConfiguracionTributaria>();
    public DbSet<AuditoriaConfiguracion> AuditoriasConfiguracion => Set<AuditoriaConfiguracion>();
    public DbSet<LlaveCriptografica> LlavesCriptograficas => Set<LlaveCriptografica>();
    public DbSet<AuditoriaLlave> AuditoriasLlave => Set<AuditoriaLlave>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActividadEconomica>(e =>
        {
            e.HasIndex(a => a.Codigo).IsUnique();
            e.Property(a => a.Codigo).HasMaxLength(10).IsRequired();
            e.Property(a => a.Descripcion).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<ConfiguracionTributaria>(e =>
        {
            e.Property(c => c.AnfitrionId).HasMaxLength(50).IsRequired();
            e.Property(c => c.TribuCr).HasMaxLength(30).IsRequired();
            e.Property(c => c.Nise).HasMaxLength(20).IsRequired();
            e.HasOne(c => c.ActividadEconomica)
             .WithMany(a => a.Configuraciones)
             .HasForeignKey(c => c.ActividadEconomicaId);
        });

        modelBuilder.Entity<AuditoriaConfiguracion>(e =>
        {
            e.Property(a => a.Campo).HasMaxLength(100);
            e.Property(a => a.ValorAnterior).HasMaxLength(500);
            e.Property(a => a.ValorNuevo).HasMaxLength(500);
            e.Property(a => a.Descripcion).HasMaxLength(500);
        });

        modelBuilder.Entity<LlaveCriptografica>(e =>
        {
            e.Property(l => l.AnfitrionId).HasMaxLength(50).IsRequired();
            e.Property(l => l.NombreArchivo).HasMaxLength(260).IsRequired();
            e.Property(l => l.ContrasenaHash).HasMaxLength(500).IsRequired();
        });

        modelBuilder.Entity<AuditoriaLlave>(e =>
        {
            e.Property(a => a.Accion).HasMaxLength(100);
            e.Property(a => a.Descripcion).HasMaxLength(500);
            e.HasOne(a => a.LlaveCriptografica)
             .WithMany(l => l.Auditorias)
             .HasForeignKey(a => a.LlaveCriptograficaId);
        });

    }
}
