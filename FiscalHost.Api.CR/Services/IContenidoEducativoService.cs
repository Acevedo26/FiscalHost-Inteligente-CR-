using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.DTOs.Communication.Requests;
using FiscalHost.Api.CR.Models.DTOs.Communication.Responses;

namespace FiscalHost.Api.CR.Services;

public interface IContenidoEducativoService
{
	Task<ContenidoEducativoDto?> ObtenerPorSlugAsync(string slug, bool incluirNoPublicados);
	Task<IEnumerable<ContenidoEducativoDto>> ListarPorCategoriaAsync(string categoria, bool incluirNoPublicados);
	Task<IEnumerable<ContenidoEducativoDto>> ListarTutorialesPrimerUsoAsync();
	Task<IEnumerable<string>> ObtenerCategoriasDisponiblesAsync();
	Task<(bool success, string? error, ContenidoEducativoDto? data)> CrearAsync(
		CreateContenidoEducativoRequest request, Guid? autorId);
	Task<(bool success, string? error, ContenidoEducativoDto? data)> ActualizarAsync(
		Guid contenidoId, ActualizarContenidoEducativoRequest request);
}