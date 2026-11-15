using FiscalHost.Api.CR.Extensions;
using FiscalHost.Api.CR.Models.DTOs.Operations.Requests;
using FiscalHost.Api.CR.Models.Enums.Operations;
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
        [FromBody] CreateReservaRequest request)
    {
        var usuarioId = User.GetUsuarioId();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Map simplified frontend payload to canonical CreateReservaRequest
        // Frontend sends: { fechaReserva, monto, huesped }
        // We derive the rest with sensible defaults for manual entry
        if (request.FechaInicio == default && request.FechaFin == default)
        {
            // If frontend sent fechaReserva as a single date, it would come in FechaInicio
            // For manual single-date reservations, treat as same-day stay
            request.FechaFin = request.FechaInicio;
        }
        
        if (request.Moneda == default)
            request.Moneda = TipoMoneda.CRC;
        
        if (request.TipoCambio == default)
            request.TipoCambio = 1m;
        
        if (request.ClasificacionFiscal == default)
            request.ClasificacionFiscal = ClasificacionFiscal.GRAVADO;
        
        if (request.PlataformaOrigen == default)
            request.PlataformaOrigen = PlataformaOrigen.DIRECTA;
        
        if (request.PeriodoFiscalAnio == default)
            request.PeriodoFiscalAnio = (short)request.FechaInicio.Year;
        
        if (request.PeriodoFiscalMes == default)
            request.PeriodoFiscalMes = (short)request.FechaInicio.Month;
        
        if (string.IsNullOrWhiteSpace(request.ReferenciaPlataforma))
            request.ReferenciaPlataforma = $"MANUAL-{Guid.NewGuid().ToString("N")[..8]}".ToUpper();
        
        if (string.IsNullOrWhiteSpace(request.Metadata))
            request.Metadata = "{}";

        var (success, error) =
            await service.RegistrarReservaAsync(usuarioId.ToString(), request);

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
        var usuarioId = User.GetUsuarioId();
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Frontend may send ISO datetime string; ensure FechaEmision is DateOnly
        // Model binding should handle ISO date (YYYY-MM-DD) portion automatically,
        // but if a full datetime arrives, the date portion is used.

        var (success, error) =
            await service.RegistrarGastoAsync(usuarioId, request);

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
        var usuarioId = User.GetUsuarioId();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (success, error) = await service.SubirComprobanteGastoAsync(usuarioId, request);

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
}
