using FiscalHost.Api.CR.Extensions;
﻿using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Mvc;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/sancion-autoliquidacion")]
public class SancionAutoliquidacionController(
	ISancionAutoliquidacionService service) : ControllerBase
{
	[HttpPost("calcular")]
	public async Task<IActionResult> Calcular([FromBody] CalcularSancionRequest request)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var usuarioId = User.GetUsuarioId();
		var (success, error, data) = await service.CalcularAsync(usuarioId, request);

		return success
			? Ok(data)
			: UnprocessableEntity(new { mensaje = error });
	}
}
