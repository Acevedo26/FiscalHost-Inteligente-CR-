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
        if (request.MontoTotal <= 0)
            return (false, "El monto debe ser mayor que cero.");

        if (request.FechaEmision > DateOnly.FromDateTime(DateTime.UtcNow))
            return (false, "La fecha del gasto no puede ser futura.");

        var gasto = new Gasto
        {
            UsuarioId = request.UsuarioId,
            PropiedadId = request.PropiedadId,
            Proveedor = request.Proveedor,
            NumeroFactura = request.NumeroFactura,
            ClaveNumericaHacienda = request.ClaveNumericaHacienda,
            MontoTotal = request.MontoTotal,
            MontoIvaSoportado = request.MontoIvaSoportado,
            MontoNeto = request.MontoNeto,
            Moneda = request.Moneda,
            TipoGasto = request.TipoGasto,
            EsDeducibleRenta = request.EsDeducibleRenta,
            EsCreditoFiscalValido = request.EsCreditoFiscalValido,
            EvidenciaUrl = request.EvidenciaUrl,
            EvidenciaNombreArchivo = request.EvidenciaNombreArchivo,
            EvidenciaTipoMime = request.EvidenciaTipoMime,
            EvidenciaTamanioBytes = request.EvidenciaTamanioBytes,
            FechaEmision = request.FechaEmision
        };

        await repository.AddGastoAsync(gasto);

        await repository.AddAuditoriaAsync(new AuditoriaOperacion
        {
            Entidad = "Gasto",
            Usuario = request.UsuarioId.ToString(),
            Accion = "CREACION",
            Descripcion = "Gasto registrado."
        });

        await repository.SaveChangesAsync();

        return (true, null);
    }
}
