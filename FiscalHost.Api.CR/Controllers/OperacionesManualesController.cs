using FiscalHost.Api.CR.Extensions;
using FiscalHost.Api.CR.Models.DTOs.Operations.Requests;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Mvc;

namespace FiscalHost.Api.CR.Controllers;

[ApiController]
[Route("api/operaciones-manuales")]
public class OperacionesManualesController(
    IOperacionManualService service) : ControllerBase
{
    [HttpPost("reserva")]
    public async Task<IActionResult> RegistrarReserva(
        [FromBody] ReservaDirectaRequest request)
    {
        request.AnfitrionId = User.GetUsuarioId().ToString();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) =
            await service.RegistrarReservaAsync(request);

        if (!success)
            return UnprocessableEntity(new { mensaje = error });

        return Ok(new
        {
            mensaje = "Reserva registrada correctamente."
        });
    }

    [HttpPost("gasto")]
    public async Task<IActionResult> RegistrarGasto(
        [FromBody] GastoOperativoRequest request)
    {
        request.UsuarioId = User.GetUsuarioId();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) =
            await service.RegistrarGastoAsync(request);

        if (!success)
            return UnprocessableEntity(new { mensaje = error });

        return Ok(new
        {
            mensaje = "Gasto operativo registrado correctamente."
        });
    }

    // ========================================================================
    // Endpoints HU-007
    // ========================================================================

    /// <summary>
    /// Sube un comprobante, extrae metadatos mediante OCR y lo guarda de forma segura.
    /// </summary>
    [HttpPost("gasto/comprobante")]
    public async Task<IActionResult> SubirComprobanteGasto(
        [FromForm] UploadComprobanteRequest request)
    {
        request.UsuarioId = User.GetUsuarioId();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (success, error) = await service.SubirComprobanteGastoAsync(request);

        if (!success) return UnprocessableEntity(new { mensaje = error });

        return Ok(new { mensaje = "Comprobante procesado y guardado exitosamente." });
    }

    /// <summary>
    /// Actualiza un gasto existente. Exige justificación por Ley 8968.
    /// </summary>
    [HttpPut("gasto/{id:guid}")]
    public async Task<IActionResult> ActualizarGasto(
        Guid id, [FromBody] UpdateGastoRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (success, error) = await service.ActualizarGastoAsync(id, request);

        if (!success) return UnprocessableEntity(new { mensaje = error });

        return Ok(new { mensaje = "Gasto actualizado y auditado correctamente." });
    }

    /// <summary>
    /// Elimina un gasto existente. Exige justificación por Ley 8968.
    /// </summary>
    [HttpDelete("gasto/{id:guid}")]
    public async Task<IActionResult> EliminarGasto(
        Guid id, [FromBody] DeleteGastoRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (success, error) = await service.EliminarGastoAsync(id, request);

        if (!success) return UnprocessableEntity(new { mensaje = error });

        return Ok(new { mensaje = "Gasto eliminado y auditado correctamente." });
    }

    [HttpGet("gastos")]
    public async Task<IActionResult> GetGastos()
    {
        var gastos = await service.GetGastosAsync(User.GetUsuarioId());
        return Ok(gastos);
    }

    [HttpGet("reservas")]
    public async Task<IActionResult> GetReservas()
    {
        var reservas = await service.GetReservasAsync(User.GetUsuarioId());
        return Ok(reservas);
    }

    /// <summary>
    /// Actualiza una reserva existente. Exige justificación por Ley 8968.
    /// </summary>
    [HttpPut("reserva/{id}")]
    public async Task<IActionResult> ActualizarReserva(
        string id, [FromBody] UpdateReservaRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (string.IsNullOrWhiteSpace(request.ObtenerRazon()))
            return UnprocessableEntity(new { mensaje = "Error: La Ley 8968 exige proporcionar una Razón de cambio o Justificación válida para modificar una reserva." });

        var (success, error) = await service.ActualizarReservaAsync(id, request);
        if (!success) return UnprocessableEntity(new { mensaje = error });

        return Ok(new { mensaje = "Reserva actualizada y auditada correctamente." });
    }

    /// <summary>
    /// Elimina una reserva existente. Exige justificación por Ley 8968.
    /// </summary>
    [HttpDelete("reserva/{id}")]
    public async Task<IActionResult> EliminarReserva(
        string id, [FromBody] DeleteReservaRequest? request = null, [FromQuery] string? razonCambio = null)
    {
        request ??= new DeleteReservaRequest { RazonCambio = razonCambio };
        if (!string.IsNullOrWhiteSpace(razonCambio) && string.IsNullOrWhiteSpace(request.RazonCambio))
        {
            request.RazonCambio = razonCambio;
        }

        if (string.IsNullOrWhiteSpace(request.ObtenerRazon()))
            return UnprocessableEntity(new { mensaje = "Error: La Ley 8968 exige proporcionar una Razón de cambio o Justificación válida para eliminar una reserva." });

        var (success, error) = await service.EliminarReservaAsync(id, request);
        if (!success) return UnprocessableEntity(new { mensaje = error });

        return Ok(new { mensaje = "Reserva eliminada y auditada correctamente." });
    }

    /// <summary>
    /// Endpoint unificado para listar operaciones manuales (ingresos/gastos).
    /// </summary>
    [HttpGet("/api/operaciones")]
    public async Task<IActionResult> GetOperaciones(
        [FromQuery] string? tipo = null,
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null)
    {
        var usuarioId = User.GetUsuarioId();
        var gastos = await service.GetGastosAsync(usuarioId);
        var reservas = await service.GetReservasAsync(usuarioId);

        var operacionesGenerales = new List<object>();

        foreach (var r in reservas)
        {
            operacionesGenerales.Add(new
            {
                id = r.ReservaId,
                tipo = "INGRESO",
                fecha = r.FechaInicio,
                monto = r.MontoColones,
                concepto = $"Reserva - {r.NombreHuesped ?? "Huésped General"}",
                huesped = r.NombreHuesped,
                fuente = r.PlataformaOrigen.ToString(),
                clasificacion = r.ClasificacionFiscal.ToString()
            });
        }

        foreach (var g in gastos)
        {
            operacionesGenerales.Add(new
            {
                id = g.GastoId,
                tipo = "GASTO",
                fecha = g.FechaEmision.ToDateTime(TimeOnly.MinValue),
                monto = g.MontoTotal,
                concepto = g.Descripcion ?? $"Gasto - {g.Proveedor}",
                proveedor = g.Proveedor,
                numeroFactura = g.NumeroFactura,
                esCreditoFiscalValido = g.EsCreditoFiscalValido
            });
        }

        if (string.Equals(tipo, "INGRESO", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(tipo, "INGRESOS", StringComparison.OrdinalIgnoreCase))
        {
            return Ok(reservas);
        }

        if (string.Equals(tipo, "GASTO", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(tipo, "GASTOS", StringComparison.OrdinalIgnoreCase))
        {
            return Ok(gastos);
        }

        return Ok(new
        {
            ingresos = reservas,
            gastos = gastos,
            operaciones = operacionesGenerales
        });
    }

    /// <summary>
    /// Endpoint unificado para crear una operación manual (ingreso o gasto).
    /// </summary>
    [HttpPost("/api/operaciones")]
    public async Task<IActionResult> CrearOperacion([FromBody] OperacionUnificadaRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var usuarioId = User.GetUsuarioId();

        if (string.Equals(request.Tipo, "GASTO", StringComparison.OrdinalIgnoreCase))
        {
            var gastoReq = new GastoOperativoRequest
            {
                UsuarioId = usuarioId,
                Proveedor = !string.IsNullOrWhiteSpace(request.Proveedor) ? request.Proveedor : (!string.IsNullOrWhiteSpace(request.Concepto) ? request.Concepto : "Proveedor General"),
                NumeroFactura = !string.IsNullOrWhiteSpace(request.NumeroFactura) ? request.NumeroFactura : $"FAC-{DateTime.UtcNow.Ticks % 1000000}",
                FechaEmision = DateOnly.FromDateTime(request.Fecha),
                Descripcion = request.Concepto,
                MontoTotal = request.Monto,
                MontoNeto = Math.Round(request.Monto / 1.13m, 2),
                MontoIvaSoportado = Math.Round(request.Monto - (request.Monto / 1.13m), 2),
                Moneda = Models.Enums.Operations.TipoMoneda.CRC,
                TipoGasto = !string.IsNullOrWhiteSpace(request.Clasificacion) ? request.Clasificacion : "General",
                EsDeducibleRenta = true,
                EsCreditoFiscalValido = true
            };

            var (success, error) = await service.RegistrarGastoAsync(gastoReq);
            if (!success)
                return UnprocessableEntity(new { mensaje = error });

            return StatusCode(StatusCodes.Status201Created, new
            {
                mensaje = "Gasto registrado correctamente.",
                data = gastoReq
            });
        }
        else
        {
            var reservaReq = new ReservaDirectaRequest
            {
                AnfitrionId = usuarioId.ToString(),
                FechaReserva = request.Fecha,
                Monto = request.Monto,
                Huesped = !string.IsNullOrWhiteSpace(request.Huesped) ? request.Huesped : (!string.IsNullOrWhiteSpace(request.Concepto) ? request.Concepto : "Huésped General")
            };

            var (success, error) = await service.RegistrarReservaAsync(reservaReq);
            if (!success)
                return UnprocessableEntity(new { mensaje = error });

            return StatusCode(StatusCodes.Status201Created, new
            {
                mensaje = "Reserva registrada correctamente.",
                data = reservaReq
            });
        }
    }
}
