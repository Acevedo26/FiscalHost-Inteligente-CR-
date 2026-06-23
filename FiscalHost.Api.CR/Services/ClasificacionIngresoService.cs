using FiscalHost.Api.CR.Models.DTOs.Operations.Requests;
using FiscalHost.Api.CR.Models.DTOs.Operations.Responses;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Enums.Operations;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public interface IClasificacionIngresoService
{
    Task<(bool success, string? error, ClasificacionIngresoResponse? data)> ClasificarAsync(
        ClasificarIngresoRequest request);

    Task<(bool success, string? error, ClasificacionIngresoResponse? data)> ReclasificarAsync(
        int id,
        ReclasificacionIngresoRequest request);

    Task<ClasificacionIngresoResponse?> GetByIdAsync(int id);
}

public class ClasificacionIngresoService(
    IClasificacionIngresoRepository repository)
    : IClasificacionIngresoService
{
    private const decimal TarifaIva = 0.13m;
    private const decimal DeduccionRenta = 0.85m;
    private const decimal TarifaRenta = 0.15m;
    private const decimal TarifaRetencionExtranjera = 0.15m;

    public async Task<(bool success, string? error, ClasificacionIngresoResponse? data)> ClasificarAsync(
        ClasificarIngresoRequest request)
    {
        var error = ValidarIngreso(request.FechaEntrada, request.FechaSalida, request.MontoBruto);
        if (error is not null)
            return (false, error, null);

        var diasEstancia = CalcularDiasEstancia(request.FechaEntrada, request.FechaSalida);
        var clasificacionIva = ClasificarIva(diasEstancia, request.HuespedResidente);

        var clasificacion = new ClasificacionIngreso
        {
            AnfitrionId = request.AnfitrionId,
            FechaEntrada = request.FechaEntrada,
            FechaSalida = request.FechaSalida,
            DiasEstancia = diasEstancia,
            MontoBruto = request.MontoBruto,
            FuenteIngreso = request.FuenteIngreso,
            TieneFacturaElectronicaNacional = request.TieneFacturaElectronicaNacional,
            HuespedResidente = request.HuespedResidente,
            ClasificacionIva = clasificacionIva
        };

        RecalcularMontos(clasificacion);

        await repository.AddAsync(clasificacion);
        await repository.SaveChangesAsync();

        return (true, null, MapToResponse(clasificacion));
    }

    public async Task<(bool success, string? error, ClasificacionIngresoResponse? data)> ReclasificarAsync(
        int id,
        ReclasificacionIngresoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Justificacion))
            return (false, "La justificacion es obligatoria para reclasificar el ingreso.", null);

        var clasificacion = await repository.GetByIdAsync(id);
        if (clasificacion is null)
            return (false, "No se encontro la clasificacion del ingreso.", null);

        var valorAnterior = clasificacion.ClasificacionIva.ToString();
        clasificacion.ClasificacionIva = request.ClasificacionIva;
        clasificacion.ReclasificadoManualmente = true;
        clasificacion.JustificacionManual = request.Justificacion;
        clasificacion.FechaActualizacion = DateTime.UtcNow;

        RecalcularMontos(clasificacion);

        await repository.AddAuditoriaAsync(new AuditoriaClasificacionIngreso
        {
            ClasificacionIngresoId = clasificacion.Id,
            UsuarioId = request.UsuarioId,
            ValorAnterior = valorAnterior,
            ValorNuevo = request.ClasificacionIva.ToString(),
            Justificacion = request.Justificacion
        });

        await repository.SaveChangesAsync();

        return (true, null, MapToResponse(clasificacion));
    }

    public async Task<ClasificacionIngresoResponse?> GetByIdAsync(int id)
    {
        var clasificacion = await repository.GetByIdAsync(id);
        return clasificacion is null ? null : MapToResponse(clasificacion);
    }

    private static string? ValidarIngreso(DateTime fechaEntrada, DateTime fechaSalida, decimal montoBruto)
    {
        if (montoBruto <= 0)
            return "El monto bruto debe ser mayor que cero.";

        if (fechaEntrada.Date > DateTime.UtcNow.Date || fechaSalida.Date > DateTime.UtcNow.Date)
            return "Las fechas de la reserva no pueden ser futuras.";

        if (fechaSalida.Date <= fechaEntrada.Date)
            return "La fecha de salida debe ser posterior a la fecha de entrada.";

        return null;
    }

    private static int CalcularDiasEstancia(DateTime fechaEntrada, DateTime fechaSalida) =>
        (fechaSalida.Date - fechaEntrada.Date).Days;

    private static ClasificacionIva ClasificarIva(int diasEstancia, bool huespedResidente)
    {
        if (diasEstancia >= 30 && huespedResidente)
            return ClasificacionIva.Exento;

        return ClasificacionIva.Gravado13;
    }

    private static void RecalcularMontos(ClasificacionIngreso clasificacion)
    {
        clasificacion.MontoIva = clasificacion.ClasificacionIva == ClasificacionIva.Gravado13
            ? clasificacion.MontoBruto * TarifaIva
            : 0;

        clasificacion.BaseImponibleRenta = clasificacion.MontoBruto * DeduccionRenta;
        clasificacion.ImpuestoRenta = clasificacion.BaseImponibleRenta * TarifaRenta;

        clasificacion.MontoRetencion =
            clasificacion.FuenteIngreso == FuenteIngreso.Extranjera &&
            !clasificacion.TieneFacturaElectronicaNacional
                ? clasificacion.MontoBruto * TarifaRetencionExtranjera
                : 0;

        clasificacion.NetoAnfitrion = clasificacion.MontoBruto
            - clasificacion.MontoIva
            - clasificacion.ImpuestoRenta
            - clasificacion.MontoRetencion;
    }

    private static ClasificacionIngresoResponse MapToResponse(ClasificacionIngreso clasificacion) => new()
    {
        Id = clasificacion.Id,
        AnfitrionId = clasificacion.AnfitrionId,
        DiasEstancia = clasificacion.DiasEstancia,
        MontoBruto = clasificacion.MontoBruto,
        FuenteIngreso = clasificacion.FuenteIngreso.ToString(),
        ClasificacionIva = clasificacion.ClasificacionIva == ClasificacionIva.Gravado13
            ? "Gravado 13% IVA"
            : "Exento de IVA",
        MontoIva = clasificacion.MontoIva,
        BaseImponibleRenta = clasificacion.BaseImponibleRenta,
        ImpuestoRenta = clasificacion.ImpuestoRenta,
        MontoRetencion = clasificacion.MontoRetencion,
        NetoAnfitrion = clasificacion.NetoAnfitrion,
        ReclasificadoManualmente = clasificacion.ReclasificadoManualmente,
        JustificacionManual = clasificacion.JustificacionManual,
        FechaActualizacion = clasificacion.FechaActualizacion
    };
}
