using FiscalHost.Api.CR.Models.DTOs;
using FiscalHost.Api.CR.Models.Entities;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public interface IOperacionManualService
{
    Task<(bool success, string? error)> RegistrarReservaAsync(
        ReservaDirectaRequest request);

    Task<(bool success, string? error)> RegistrarGastoAsync(
        GastoOperativoRequest request);
}

public class OperacionManualService(
    IOperacionManualRepository repository)
    : IOperacionManualService
{
    public async Task<(bool success, string? error)> RegistrarReservaAsync(
        ReservaDirectaRequest request)
    {
        if (request.Monto <= 0)
            return (false, "El monto debe ser mayor que cero.");

        if (request.FechaReserva > DateTime.UtcNow)
            return (false, "La fecha de reserva no puede ser futura.");

        var reserva = new ReservaDirecta
        {
            AnfitrionId = request.AnfitrionId,
            FechaReserva = request.FechaReserva,
            Monto = request.Monto,
            Huesped = request.Huesped
        };

        await repository.AddReservaAsync(reserva);

        await repository.AddAuditoriaAsync(new AuditoriaOperacion
        {
            Entidad = "ReservaDirecta",
            Usuario = request.AnfitrionId,
            Accion = "CREACION",
            Descripcion = "Reserva directa registrada."
        });

        await repository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool success, string? error)> RegistrarGastoAsync(
        GastoOperativoRequest request)
    {
        if (request.Monto <= 0)
            return (false, "El monto debe ser mayor que cero.");

        if (request.FechaGasto > DateTime.UtcNow)
            return (false, "La fecha del gasto no puede ser futura.");

        var gasto = new GastoOperativo
        {
            AnfitrionId = request.AnfitrionId,
            Proveedor = request.Proveedor,
            NumeroFactura = request.NumeroFactura,
            Monto = request.Monto,
            ComprobanteUrl = request.ComprobanteUrl,
            FechaGasto = request.FechaGasto
        };

        await repository.AddGastoAsync(gasto);

        await repository.AddAuditoriaAsync(new AuditoriaOperacion
        {
            Entidad = "GastoOperativo",
            Usuario = request.AnfitrionId,
            Accion = "CREACION",
            Descripcion = "Gasto operativo registrado."
        });

        await repository.SaveChangesAsync();

        return (true, null);
    }
}