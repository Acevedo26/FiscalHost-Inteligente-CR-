using Microsoft.EntityFrameworkCore;
using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.Audit;
using FiscalHost.Api.CR.Models.Entities.Operations;

namespace FiscalHost.Api.CR.Repositories;

public interface IOperacionManualRepository
{
    Task AddReservaAsync(ReservaDirecta reserva);

    Task AddGastoAsync(Gasto gasto);
    
    Task<Gasto?> GetGastoByIdAsync(Guid id);

    Task UpdateGastoAsync(Gasto gasto);

    Task DeleteGastoAsync(Gasto gasto);

    Task<bool> ExisteGastoDuplicadoAsync(string proveedor, string numeroFactura);

    Task AddAuditoriaAsync(AuditoriaOperacion auditoria);

    Task SaveChangesAsync();
}

public class OperacionManualRepository(AppDbContext context) : IOperacionManualRepository
{
    public async Task AddReservaAsync(ReservaDirecta reserva)
    {
        await context.ReservasDirectas.AddAsync(reserva);
    }

    public async Task AddGastoAsync(Gasto gasto)
    {
        await context.Gastos.AddAsync(gasto);
    }

    public async Task<Gasto?> GetGastoByIdAsync(Guid id)
    {
        return await context.Gastos.FirstOrDefaultAsync(g => g.GastoId == id);
    }

    public Task UpdateGastoAsync(Gasto gasto)
    {
        context.Gastos.Update(gasto);
        return Task.CompletedTask;
    }

    public Task DeleteGastoAsync(Gasto gasto)
    {
        context.Gastos.Remove(gasto);
        return Task.CompletedTask;
    }

    // ========================================================================
    // Regla de Negocio: Evitar Comprobantes Duplicados.
    // Verifica en la base de datos si ya existe un gasto con el mismo proveedor
    // y el mismo número de factura.
    // ========================================================================
    public async Task<bool> ExisteGastoDuplicadoAsync(string proveedor, string numeroFactura)
    {
        // Convertimos a minúsculas para una comparación case-insensitive
        return await context.Gastos.AnyAsync(g => 
            g.Proveedor.ToLower() == proveedor.ToLower() && 
            g.NumeroFactura == numeroFactura);
    }

    public async Task AddAuditoriaAsync(AuditoriaOperacion auditoria)
    {
        await context.AuditoriasOperacion.AddAsync(auditoria);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
