using System.Text.Json;
using System.Text.RegularExpressions;
using FiscalHost.Api.CR.Models.DTOs.Identity.Requests;
using FiscalHost.Api.CR.Models.DTOs.Identity.Responses;
using FiscalHost.Api.CR.Models.Entities.Audit;
using FiscalHost.Api.CR.Models.Entities.Identity;
using FiscalHost.Api.CR.Models.Enums.Audit;
using FiscalHost.Api.CR.Models.Enums.Identity;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public interface IAccesoContadorService
{
    Task<(bool success, string? error, AccesoContadorResponse? data)> InvitarAsync(InvitarContadorRequest request);
    Task<(bool success, string? error)> RevocarAsync(Guid accesoId, RevocarAccesoContadorRequest request);
    Task<(bool autorizado, string mensaje)> ValidarPermisoAsync(Guid anfitrionId, string correoContador, string permiso);
    Task<List<AccesoContadorResponse>> GetByAnfitrionAsync(Guid anfitrionId);
    Task<int> ProcesarExpiracionesAsync();
}

public class AccesoContadorService(
    IAccesoContadorRepository repository,
    INotificacionService notificaciones)
    : IAccesoContadorService
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public async Task<(bool success, string? error, AccesoContadorResponse? data)> InvitarAsync(
        InvitarContadorRequest request)
    {
        if (request.AnfitrionId == Guid.Empty)
            return (false, "El anfitrion es obligatorio.", null);

        if (!EmailRegex.IsMatch(request.CorreoContador))
            return (false, "El correo del contador no posee un formato valido.", null);

        if (request.FechaExpiracion.HasValue && request.FechaExpiracion <= DateTimeOffset.UtcNow)
            return (false, "La fecha de expiracion debe ser futura.", null);

        var anfitrion = await repository.GetUsuarioAsync(request.AnfitrionId);
        if (anfitrion is null)
            return (false, "No se encontro el anfitrion.", null);

        var existente = await repository.GetActivoPorCorreoAsync(
            request.AnfitrionId,
            request.CorreoContador);

        if (existente is not null)
            return (false, "El contador ya posee un acceso activo.", null);

        var contador = await repository.GetUsuarioPorCorreoAsync(request.CorreoContador);
        var acceso = new AccesoContador
        {
            AccesoId = Guid.NewGuid(),
            AnfitrionId = request.AnfitrionId,
            ContadorId = contador?.UsuarioId,
            CorreoContador = request.CorreoContador.Trim().ToLowerInvariant(),
            Permisos = JsonSerializer.Serialize(new PermisosContador
            {
                PuedeVerIngresos = request.PuedeVerIngresos,
                PuedeVerGastos = request.PuedeVerGastos,
                PuedeGenerarReportes = request.PuedeGenerarReportes
            }),
            FechaInvitacion = DateTimeOffset.UtcNow,
            FechaExpiracion = request.FechaExpiracion,
            Estado = "ACTIVO"
        };

        await repository.AddAsync(acceso);
        await repository.AddAuditoriaAsync(BuildAuditoria(
            anfitrion,
            OperacionAuditoria.INSERT,
            "acceso_contador",
            acceso.AccesoId,
            null,
            new
            {
                acceso.AccesoId,
                acceso.AnfitrionId,
                acceso.ContadorId,
                acceso.CorreoContador,
                acceso.Permisos,
                acceso.FechaExpiracion,
                acceso.Estado
            },
            "Invitacion segura de contador."));
        await repository.SaveChangesAsync();

        await notificaciones.NotificarAsync(
            request.AnfitrionId.ToString(),
            $"Invitacion enviada al contador {acceso.CorreoContador}.");

        return (true, null, MapToResponse(acceso));
    }

    public async Task<(bool success, string? error)> RevocarAsync(
        Guid accesoId,
        RevocarAccesoContadorRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Justificacion))
            return (false, "La justificacion es obligatoria para revocar el acceso.");

        var acceso = await repository.GetByIdAsync(accesoId);
        if (acceso is null || acceso.AnfitrionId != request.AnfitrionId)
            return (false, "No se encontro un acceso autorizado para revocar.");

        if (acceso.Estado != "ACTIVO")
            return (false, "El acceso ya no se encuentra activo.");

        var anterior = new
        {
            acceso.Estado,
            acceso.FechaRevocacion
        };

        acceso.Estado = "REVOCADO";
        acceso.FechaRevocacion = DateTimeOffset.UtcNow;

        var anfitrion = await repository.GetUsuarioAsync(request.AnfitrionId);
        await repository.AddAuditoriaAsync(BuildAuditoria(
            anfitrion,
            OperacionAuditoria.UPDATE,
            "acceso_contador",
            acceso.AccesoId,
            anterior,
            new { acceso.Estado, acceso.FechaRevocacion },
            request.Justificacion));
        await repository.SaveChangesAsync();

        await notificaciones.NotificarAsync(
            request.AnfitrionId.ToString(),
            $"Acceso revocado para el contador {acceso.CorreoContador}.");

        return (true, null);
    }

    public async Task<(bool autorizado, string mensaje)> ValidarPermisoAsync(
        Guid anfitrionId,
        string correoContador,
        string permiso)
    {
        var acceso = await repository.GetActivoPorCorreoAsync(anfitrionId, correoContador);

        if (acceso is null)
            return (false, "Permisos insuficientes: no existe acceso activo.");

        if (acceso.FechaExpiracion.HasValue && acceso.FechaExpiracion <= DateTimeOffset.UtcNow)
            return (false, "Permisos insuficientes: el acceso se encuentra expirado.");

        var permisos = LeerPermisos(acceso.Permisos);
        var autorizado = permiso.Trim().ToUpperInvariant() switch
        {
            "INGRESOS" => permisos.PuedeVerIngresos,
            "GASTOS" => permisos.PuedeVerGastos,
            "REPORTES" => permisos.PuedeGenerarReportes,
            _ => false
        };

        return autorizado
            ? (true, "Acceso autorizado.")
            : (false, "Permisos insuficientes para consultar la informacion solicitada.");
    }

    public async Task<List<AccesoContadorResponse>> GetByAnfitrionAsync(Guid anfitrionId)
    {
        var accesos = await repository.GetByAnfitrionAsync(anfitrionId);
        return accesos.Select(MapToResponse).ToList();
    }

    public async Task<int> ProcesarExpiracionesAsync()
    {
        var ahora = DateTimeOffset.UtcNow;
        var porVencer = await repository.GetPorVencerAsync(ahora, ahora.AddDays(3));

        foreach (var acceso in porVencer)
        {
            await notificaciones.NotificarAsync(
                acceso.AnfitrionId.ToString(),
                $"El acceso del contador {acceso.CorreoContador} vence pronto.");
        }

        var expirados = await repository.GetExpiradosAsync(ahora);

        foreach (var acceso in expirados)
        {
            acceso.Estado = "EXPIRADO";
            await repository.AddAuditoriaAsync(new AuditoriaOperacion
            {
                UsuarioId = acceso.AnfitrionId,
                Operacion = OperacionAuditoria.UPDATE,
                TablaAfectada = "acceso_contador",
                RegistroId = acceso.AccesoId,
                OldValues = JsonSerializer.Serialize(new { Estado = "ACTIVO" }),
                NewValues = JsonSerializer.Serialize(new { Estado = "EXPIRADO" }),
                CamposModificados = ["Estado"],
                Justificacion = "Expiracion automatica del acceso temporal."
            });
        }

        if (expirados.Any())
            await repository.SaveChangesAsync();

        return expirados.Count;
    }

    private static AuditoriaOperacion BuildAuditoria(
        Usuario? usuario,
        OperacionAuditoria operacion,
        string tabla,
        Guid registroId,
        object? oldValues,
        object newValues,
        string justificacion) => new()
    {
        UsuarioId = usuario?.UsuarioId,
        CorreoUsuario = usuario?.CorreoElectronico,
        RolUsuario = usuario?.RolPrincipal ?? RolUsuario.ANFITRION,
        Operacion = operacion,
        TablaAfectada = tabla,
        RegistroId = registroId,
        OldValues = oldValues is null ? null : JsonSerializer.Serialize(oldValues),
        NewValues = JsonSerializer.Serialize(newValues),
        CamposModificados = ["Estado", "Permisos", "FechaExpiracion"],
        Justificacion = justificacion
    };

    private static AccesoContadorResponse MapToResponse(AccesoContador acceso)
    {
        var permisos = LeerPermisos(acceso.Permisos);
        return new AccesoContadorResponse
        {
            AccesoId = acceso.AccesoId,
            AnfitrionId = acceso.AnfitrionId,
            ContadorId = acceso.ContadorId,
            CorreoContador = acceso.CorreoContador,
            Estado = acceso.Estado,
            FechaInvitacion = acceso.FechaInvitacion,
            FechaExpiracion = acceso.FechaExpiracion,
            FechaRevocacion = acceso.FechaRevocacion,
            PuedeVerIngresos = permisos.PuedeVerIngresos,
            PuedeVerGastos = permisos.PuedeVerGastos,
            PuedeGenerarReportes = permisos.PuedeGenerarReportes
        };
    }

    private static PermisosContador LeerPermisos(string permisosJson)
    {
        return JsonSerializer.Deserialize<PermisosContador>(permisosJson)
            ?? new PermisosContador();
    }

    private sealed class PermisosContador
    {
        public bool PuedeVerIngresos { get; set; }
        public bool PuedeVerGastos { get; set; }
        public bool PuedeGenerarReportes { get; set; }
    }
}
