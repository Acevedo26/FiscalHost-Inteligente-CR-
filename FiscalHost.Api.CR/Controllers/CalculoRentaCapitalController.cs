using FiscalHost.Api.CR.Extensions;
﻿using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Mvc;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/renta-capital")]
public class CalculoRentaCapitalController(
	ICalculoRentaCapitalService service) : ControllerBase
{
	[HttpPost("calcular")]
	public async Task<IActionResult> Calcular([FromBody] CalcularRentaCapitalRequest request)
	{
		var usuarioId = User.GetUsuarioId();
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var (success, error, data) = await service.CalcularAsync(usuarioId, request);

		return success
			? Ok(data)
			: UnprocessableEntity(new { mensaje = error });
	}

	[HttpPost("simular")]
	public async Task<IActionResult> Simular([FromBody] SimularRegimenRequest request)
	{
		var usuarioId = User.GetUsuarioId();
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var (success, error, data) = await service.SimularAsync(usuarioId, request);

		return success
			? Ok(data)
			: UnprocessableEntity(new { mensaje = error });
	}

	[HttpPost("cambiar-regimen")]
	public async Task<IActionResult> CambiarRegimen([FromBody] CambiarRegimenTributarioRequest request)
	{
		var usuarioId = User.GetUsuarioId();
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var (success, error, regimenActual) = await service.CambiarRegimenAsync(usuarioId, request);

		return success
			? Ok(new { regimenActual })
			: UnprocessableEntity(new { mensaje = error });
	}
}
