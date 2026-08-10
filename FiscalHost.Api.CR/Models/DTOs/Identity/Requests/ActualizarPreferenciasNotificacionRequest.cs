using System.ComponentModel.DataAnnotations;
using FiscalHost.Api.CR.Models.Enums.Communication;

namespace FiscalHost.Api.CR.Models.DTOs.Identity.Requests;

// ========================================================================
// DTO de Request (Entrada): Contiene, únicamente, los datos que el usuario
// o frontend envía a la API al realizar una petición, sin incluir
// identificadores generados ni campos de auditoría internos.
// ========================================================================


public class ActualizarPreferenciasNotificacionRequest
{
	[Required]
	public CanalNotificacion CanalAlertas { get; set; }
}