using System.Text;
using FiscalHost.Api.CR.Models.DTOs.Audit.Requests;
using FiscalHost.Api.CR.Models.DTOs.Audit.Responses;
using FiscalHost.Api.CR.Models.Entities.Audit;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public interface IAuditoriaInalterableService
{
    Task<(bool success, string? error, AuditoriaOperacionResponse? data)> RegistrarAsync(
        RegistrarAuditoriaRequest request);

    Task<List<AuditoriaOperacionResponse>> ConsultarHistorialAsync(
        Guid? usuarioId,
        string? tablaAfectada,
        Guid? registroId);

    Task<ExportacionAuditoriaResponse> ExportarHistorialAsync(
        Guid? usuarioId,
        string? tablaAfectada,
        Guid? registroId);
}

public class AuditoriaInalterableService(
    IAuditoriaInalterableRepository repository)
    : IAuditoriaInalterableService
{
    public async Task<(bool success, string? error, AuditoriaOperacionResponse? data)> RegistrarAsync(
        RegistrarAuditoriaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TablaAfectada))
            return (false, "La tabla afectada es obligatoria.", null);

        if (request.EsCampoSensible && string.IsNullOrWhiteSpace(request.Justificacion))
            return (false, "La justificacion es obligatoria para modificar campos sensibles.", null);

        var auditoria = new AuditoriaOperacion
        {
            AuditId = Guid.NewGuid(),
            UsuarioId = request.UsuarioId,
            CorreoUsuario = request.CorreoUsuario,
            RolUsuario = request.RolUsuario,
            Operacion = request.Operacion,
            TablaAfectada = request.TablaAfectada.Trim(),
            RegistroId = request.RegistroId,
            OldValues = request.OldValues,
            NewValues = request.NewValues,
            CamposModificados = request.CamposModificados,
            Justificacion = request.Justificacion,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await repository.AddAsync(auditoria);
        await repository.SaveChangesAsync();

        return (true, null, MapToResponse(auditoria));
    }

    public async Task<List<AuditoriaOperacionResponse>> ConsultarHistorialAsync(
        Guid? usuarioId,
        string? tablaAfectada,
        Guid? registroId)
    {
        var historial = await repository.GetHistorialAsync(usuarioId, tablaAfectada, registroId);
        return historial.Select(MapToResponse).ToList();
    }

    public async Task<ExportacionAuditoriaResponse> ExportarHistorialAsync(
        Guid? usuarioId,
        string? tablaAfectada,
        Guid? registroId)
    {
        var historial = await repository.GetHistorialAsync(usuarioId, tablaAfectada, registroId);

        if (!historial.Any())
        {
            return new ExportacionAuditoriaResponse
            {
                Success = false,
                Mensaje = "No existen registros de auditoria para exportar."
            };
        }

        var sb = new StringBuilder();
        sb.AppendLine("AuditId,UsuarioId,CorreoUsuario,Operacion,TablaAfectada,RegistroId,CamposModificados,Justificacion,CreatedAt");

        foreach (var item in historial)
        {
            sb.AppendLine(string.Join(",",
                item.AuditId,
                item.UsuarioId,
                EscapeCsv(item.CorreoUsuario ?? ""),
                item.Operacion,
                EscapeCsv(item.TablaAfectada),
                item.RegistroId,
                EscapeCsv(item.CamposModificados is null ? "" : string.Join("|", item.CamposModificados)),
                EscapeCsv(item.Justificacion ?? ""),
                item.CreatedAt.ToString("O")));
        }

        return new ExportacionAuditoriaResponse
        {
            Success = true,
            Mensaje = "Historial de auditoria exportado correctamente.",
            NombreArchivo = $"historial_auditoria_{DateTimeOffset.UtcNow:yyyyMMddHHmmss}.csv",
            ContenidoBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()))
        };
    }

    private static AuditoriaOperacionResponse MapToResponse(AuditoriaOperacion auditoria) => new()
    {
        AuditId = auditoria.AuditId,
        UsuarioId = auditoria.UsuarioId,
        CorreoUsuario = auditoria.CorreoUsuario,
        RolUsuario = auditoria.RolUsuario?.ToString(),
        Operacion = auditoria.Operacion.ToString(),
        TablaAfectada = auditoria.TablaAfectada,
        RegistroId = auditoria.RegistroId,
        OldValues = auditoria.OldValues,
        NewValues = auditoria.NewValues,
        CamposModificados = auditoria.CamposModificados,
        Justificacion = auditoria.Justificacion,
        CreatedAt = auditoria.CreatedAt
    };

    private static string EscapeCsv(string value) =>
        $"\"{value.Replace("\"", "\"\"")}\"";
}
