using Microsoft.AspNetCore.Http;

namespace FiscalHost.Api.CR.DTOs.Operations;

public class ImportacionRequest
{
	public IFormFile Archivo { get; set; } = default!;

	public Guid UsuarioId { get; set; }
}