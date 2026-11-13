using FiscalHost.Api.CR.Models.DTOs.Operations.Requests;
using FiscalHost.Api.CR.Models.DTOs.Operations.Responses;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Models.Entities.Audit;
using System.Text.Json;

namespace FiscalHost.Api.CR.Services;

public class OperacionManualServiceAuditDecorator(
    IOperacionManualService innerService,
    IOperacionManualRepository repository) : IOperacionManualService
{
    public async Task<(bool success, string? error)> RegistrarReservaAsync(ReservaDirectaRequest request)
    {
        var result = await innerService.RegistrarReservaAsync(request);
        if (result.success)
        {
            await repository.AddAuditoriaAsync(new AuditoriaOperacion
            {
                UsuarioId = Guid.Parse(request.AnfitrionId),
                Operacion = OperacionAuditoria.INSERT,
                TablaAfectada = "reserva_directa",
                NewValues = JsonSerializer.Serialize(request)
            });
            await repository.SaveChangesAsync();
        }
        return result;
    }

    public async Task<(bool success, string? error)> RegistrarGastoAsync(GastoOperativoRequest request)
    {
        var result = await innerService.RegistrarGastoAsync(request);
        if (result.success)
        {
            await repository.AddAuditoriaAsync(new AuditoriaOperacion
            {
                UsuarioId = request.UsuarioId,
                Operacion = OperacionAuditoria.INSERT,
                TablaAfectada = "gasto",
                NewValues = JsonSerializer.Serialize(request)
            });
            await repository.SaveChangesAsync();
        }
        return result;
    }

    public async Task<(bool success, string? error)> SubirComprobanteGastoAsync(UploadComprobanteRequest request)
    {
        var result = await innerService.SubirComprobanteGastoAsync(request);
        if (result.success)
        {
            await repository.AddAuditoriaAsync(new AuditoriaOperacion
            {
                UsuarioId = request.UsuarioId,
                Operacion = OperacionAuditoria.INSERT,
                TablaAfectada = "gasto",
                NewValues = "Carga de comprobante: " + request.Comprobante.FileName
            });
            await repository.SaveChangesAsync();
        }
        return result;
    }

    public async Task<(bool success, string? error)> ActualizarGastoAsync(Guid id, UpdateGastoRequest request)
    {
        var result = await innerService.ActualizarGastoAsync(id, request);
        // El servicio interno ya maneja la auditoria para UPDATE por la complejidad de OldValues, pero podríamos moverlo aquí.
        // Para no duplicar código que ya existe, solo pasamos la llamada.
        return result;
    }

    public async Task<(bool success, string? error)> EliminarGastoAsync(Guid id, DeleteGastoRequest request)
    {
        return await innerService.EliminarGastoAsync(id, request);
    }

    public async Task<(bool success, string? error)> ActualizarReservaAsync(string id, UpdateReservaRequest request)
    {
        return await innerService.ActualizarReservaAsync(id, request);
    }

    public async Task<(bool success, string? error)> EliminarReservaAsync(string id, DeleteReservaRequest request)
    {
        return await innerService.EliminarReservaAsync(id, request);
    }

    public async Task<IEnumerable<GastoDto>> GetGastosAsync(Guid usuarioId)
    {
        return await innerService.GetGastosAsync(usuarioId);
    }

    public async Task<IEnumerable<ReservaDto>> GetReservasAsync(Guid usuarioId)
    {
        return await innerService.GetReservasAsync(usuarioId);
    }
}
