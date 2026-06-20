

namespace FiscalHost.Api.CR.Repositories;

public interface IOperacionManualRepository
{
    Task AddReservaAsync(ReservaDirecta reserva);

    Task AddGastoAsync(GastoOperativo gasto);

    Task AddAuditoriaAsync(AuditoriaOperacion auditoria);

    Task SaveChangesAsync();
}

public class OperacionManualRepository : IOperacionManualRepository
{
    public Task AddReservaAsync(ReservaDirecta reserva)
    {
        return Task.CompletedTask;
    }

    public Task AddGastoAsync(GastoOperativo gasto)
    {
        return Task.CompletedTask;
    }

    public Task AddAuditoriaAsync(AuditoriaOperacion auditoria)
    {
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}
