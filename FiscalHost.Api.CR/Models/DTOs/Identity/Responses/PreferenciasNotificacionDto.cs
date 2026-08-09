using FiscalHost.Api.CR.Models.Enums.Communication;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Responses;

// ========================================================================
// DTO de Response (Salida): Contiene la información formateada que la API
// le devuelve al frontend para mostrar en pantalla, ocultando, de esta
// manera, cualquier dato sensible.
// ========================================================================


public class PreferenciasNotificacionDto
{
	public CanalNotificacion CanalAlertas { get; set; }
}