

namespace FiscalHost.Api.CR.Repositories;

public interface IOperacionManualRepository
{
    Task AddReservaAsync(ReservaDirecta reserva);

    Task AddGastoAsync(FiscalHost.Api.CR.Models.Entities.Operations.Gasto gasto);

    Task AddAuditoriaAsync(AuditoriaOperacion auditoria);

    Task SaveChangesAsync();
}

public class OperacionManualRepository : IOperacionManualRepository
{
    public Task AddReservaAsync(ReservaDirecta reserva)
    {
        return Task.CompletedTask;
    }

    public Task AddGastoAsync(FiscalHost.Api.CR.Models.Entities.Operations.Gasto gasto)
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
