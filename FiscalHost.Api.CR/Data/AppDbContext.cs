using FiscalHost.Api.CR.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ActividadEconomica> ActividadesEconomicas => Set<ActividadEconomica>();
    public DbSet<ConfiguracionTributaria> ConfiguracionesTributarias => Set<ConfiguracionTributaria>();
    public DbSet<AuditoriaConfiguracion> AuditoriasConfiguracion => Set<AuditoriaConfiguracion>();

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
            e.Property(c => c.TribuCr).HasMaxLength(50).IsRequired();
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

    }
}
