using System.ComponentModel.DataAnnotations;

namespace FiscalHost.Api.CR.Models.DTOs.Communication.Requests;

// ========================================================================
// DTO de Request (Entrada): Contiene, únicamente, los datos que el usuario
// o frontend envía a la API al realizar una petición, sin incluir
// identificadores generados ni campos de auditoría internos.
// ========================================================================


public class ActualizarContenidoEducativoRequest
{
	[Required]
	public string Titulo { get; set; } = string.Empty;

	[Required]
	public string ContenidoMarkdown { get; set; } = string.Empty;

	public bool Publicado { get; set; }

	// RF-019 - Escenario "Actualización de contenido": si es true, se notifica
	// a los usuarios activos que el contenido cambió.
	public bool NotificarUsuarios { get; set; } = true;
}