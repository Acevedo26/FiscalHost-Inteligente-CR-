namespace FiscalHost.Api.CR.Services;

public class NotificacionService(ILogger<NotificacionService> logger) : INotificacionService
{
    // DEUDA TÉCNICA (HU-003): el envío real de email está pendiente de implementar.
    // Integrar con proveedor transaccional (AWS SES, SendGrid).
    // Issue de seguimiento: pendiente de crear en GitHub Projects bajo etiqueta `feature`.
    public Task NotificarAsync(string anfitrionId, string mensaje)
    {
        logger.LogInformation("[NOTIFICACIÓN PENDIENTE] AnfitrionId={AnfitrionId} | {Mensaje}", anfitrionId, mensaje);
        return Task.CompletedTask;
    }
}
