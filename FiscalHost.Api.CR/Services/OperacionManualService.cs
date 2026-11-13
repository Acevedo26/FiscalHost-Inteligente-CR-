using System;
using System.Text.Json;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.DTOs.Operations.Requests;
using FiscalHost.Api.CR.Models.Entities.Audit;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Enums.Audit;
using FiscalHost.Api.CR.Models.Enums.Operations;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public interface IOperacionManualService
{
    Task<(bool success, string? error)> RegistrarReservaAsync(ReservaDirectaRequest request);

    Task<(bool success, string? error)> RegistrarGastoAsync(GastoOperativoRequest request);

    // Nuevos métodos de la HU-007
    Task<(bool success, string? error)> SubirComprobanteGastoAsync(UploadComprobanteRequest request);
    Task<(bool success, string? error)> ActualizarGastoAsync(Guid id, UpdateGastoRequest request);
    Task<(bool success, string? error)> EliminarGastoAsync(Guid id, DeleteGastoRequest request);
    Task<(bool success, string? error)> ActualizarReservaAsync(string id, UpdateReservaRequest request);
    Task<(bool success, string? error)> EliminarReservaAsync(string id, DeleteReservaRequest request);
    Task<IEnumerable<FiscalHost.Api.CR.Models.DTOs.Operations.Responses.GastoDto>> GetGastosAsync(Guid usuarioId);
    Task<IEnumerable<FiscalHost.Api.CR.Models.DTOs.Operations.Responses.ReservaDto>> GetReservasAsync(Guid usuarioId);
}

public class OperacionManualService(
    IOperacionManualRepository repository,
    IBlobStorageService blobStorageService,
    IOcrService ocrService)
    : IOperacionManualService
{
    public async Task<(bool success, string? error)> RegistrarReservaAsync(ReservaDirectaRequest request)
    {
        if (request.Monto <= 0) return (false, "El monto debe ser mayor que cero.");
        if (request.FechaReserva > DateTime.UtcNow) return (false, "La fecha de reserva no puede ser futura.");

        var reserva = new ReservaDirecta
        {
            AnfitrionId = request.AnfitrionId,
            FechaReserva = request.FechaReserva,
            Monto = request.Monto,
            Huesped = request.Huesped
        };

        await repository.AddReservaAsync(reserva);
        await repository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool success, string? error)> RegistrarGastoAsync(GastoOperativoRequest request)
    {
        if (request.MontoTotal <= 0) return (false, "El monto debe ser mayor que cero.");
        if (request.FechaEmision > DateOnly.FromDateTime(DateTime.UtcNow)) return (false, "La fecha del gasto no puede ser futura.");

        var gasto = new Gasto
        {
            UsuarioId = request.UsuarioId,
            PropiedadId = request.PropiedadId,
            Proveedor = request.Proveedor,
            NumeroFactura = request.NumeroFactura,
            ClaveNumericaHacienda = request.ClaveNumericaHacienda,
            Descripcion = request.Descripcion,
            MontoTotal = request.MontoTotal,
            MontoIvaSoportado = request.MontoIvaSoportado,
            MontoNeto = request.MontoNeto,
            Moneda = request.Moneda,
            TipoGasto = request.TipoGasto,
            EsDeducibleRenta = request.EsDeducibleRenta,
            EsCreditoFiscalValido = request.EsCreditoFiscalValido,
            EvidenciaUrl = request.EvidenciaUrl,
            EvidenciaNombreArchivo = request.EvidenciaNombreArchivo,
            EvidenciaTipoMime = request.EvidenciaTipoMime,
            EvidenciaTamanioBytes = request.EvidenciaTamanioBytes,
            FechaEmision = request.FechaEmision
        };

        await repository.AddGastoAsync(gasto);
        await repository.SaveChangesAsync();
        return (true, null);
    }

    // ========================================================================
    // Implementación de HU-007: Subir Comprobante (Azure Blob + OCR)
    // ========================================================================
    public async Task<(bool success, string? error)> SubirComprobanteGastoAsync(UploadComprobanteRequest request)
    {
        if (request.Comprobante == null || request.Comprobante.Length == 0)
        {
            return (false, "El archivo proporcionado está vacío o no es válido.");
        }

        try
        {
            // 1. Copiar a MemoryStream para poder reutilizarlo en OCR
            using var ms = new MemoryStream();
            await request.Comprobante.CopyToAsync(ms);

            // 2. Almacenamiento local en disco
            ms.Position = 0;
            string fileUrl = await blobStorageService.UploadAsync(
                ms,
                request.Comprobante.FileName,
                request.Comprobante.ContentType);

            // 3. Extracción de Metadatos mediante OCR
            ms.Position = 0;
            var ocrResult = await ocrService.ExtractMetadataAsync(ms, request.Comprobante.ContentType);

            // 3. Validación de Duplicados
            if (ocrResult.Exitoso && !string.IsNullOrEmpty(ocrResult.Proveedor) && !string.IsNullOrEmpty(ocrResult.NumeroFactura))
            {
                bool esDuplicado = await repository.ExisteGastoDuplicadoAsync(ocrResult.Proveedor, ocrResult.NumeroFactura);
                if (esDuplicado)
                {
                    return (false, $"El sistema bloqueó el registro: Ya existe la factura {ocrResult.NumeroFactura} para el proveedor {ocrResult.Proveedor}.");
                }
            }

            // 4. Lógica de Crédito Fiscal y Régimen de Utilidades
            // Un gasto es "Crédito Fiscal Válido" automáticamente si el OCR fue exitoso y encontró datos mínimos.
            bool esCreditoValido = ocrResult.Exitoso;

            if (!ocrResult.Exitoso || !ocrResult.MontoTotal.HasValue)
            {
                throw new FiscalHost.Api.CR.Exceptions.BusinessRuleValidationException(
                    "El OCR no pudo extraer los datos del comprobante. Complete los campos manualmente.",
                    new
                    {
                        exitoOcr = false,
                        evidenciaUrl = fileUrl,
                        datosExtraidos = ocrResult
                    }
                );
            }

            var gasto = new Gasto
            {
                UsuarioId = request.UsuarioId,
                PropiedadId = request.PropiedadId,
                Proveedor = ocrResult.Proveedor ?? "PENDIENTE",
                NumeroFactura = ocrResult.NumeroFactura ?? "PENDIENTE",
                FechaEmision = ocrResult.FechaEmision ?? DateOnly.FromDateTime(DateTime.UtcNow),
                MontoTotal = ocrResult.MontoTotal!.Value,
                EvidenciaUrl = fileUrl,
                EvidenciaNombreArchivo = request.Comprobante.FileName,
                EvidenciaTipoMime = request.Comprobante.ContentType,
                EvidenciaTamanioBytes = request.Comprobante.Length,
                EstadoOcr = ocrResult.Exitoso ? EstadoOcr.PROCESADO : EstadoOcr.ILEGIBLE,
                DatosExtraidosOcr = JsonSerializer.Serialize(ocrResult),
                EsCreditoFiscalValido = esCreditoValido,
                EsDeducibleRenta = esCreditoValido, // Se marca como deducible automáticamente si es válido.
                FuenteRegistro = FuenteRegistro.MANUAL,
                Moneda = TipoMoneda.CRC, // Por defecto Colones,
                TipoGasto = "General",
                EstadoValidacion = EstadoValidacion.PENDIENTE
            };

            await repository.AddGastoAsync(gasto);

            // Registrar Auditoría Inicial
            await repository.AddAuditoriaAsync(new AuditoriaOperacion
            {
                UsuarioId = request.UsuarioId,
                Operacion = OperacionAuditoria.INSERT,
                TablaAfectada = "gasto",
                NewValues = JsonSerializer.Serialize(new { gasto.Proveedor, gasto.NumeroFactura, gasto.EstadoOcr })
            });

            await repository.SaveChangesAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Ocurrió un error interno al procesar el archivo: {ex.Message}");
        }
    }

    // ========================================================================
    // Implementación de HU-007: Modificación con Auditoría Estricta (Ley 8968)
    // ========================================================================
    public async Task<(bool success, string? error)> ActualizarGastoAsync(Guid id, UpdateGastoRequest request)
    {
        // 1. Validación Estricta de la Justificación
        if (string.IsNullOrWhiteSpace(request.Justificacion))
        {
            return (false, "Error: La Ley 8968 exige proporcionar una Justificación válida para modificar un comprobante.");
        }

        var gasto = await repository.GetGastoByIdAsync(id);
        if (gasto == null) return (false, "Gasto no encontrado.");

        // Guardamos el estado anterior para la bitácora
        string valorAnterior = JsonSerializer.Serialize(new 
        { 
            gasto.MontoTotal, gasto.Proveedor, gasto.NumeroFactura, gasto.EsCreditoFiscalValido
        });

        // Aplicamos los cambios
        gasto.Proveedor = request.Proveedor;
        gasto.NumeroFactura = request.NumeroFactura;
        gasto.FechaEmision = request.FechaEmision;
        gasto.MontoTotal = request.MontoTotal;
        gasto.MontoIvaSoportado = request.MontoIvaSoportado;
        gasto.MontoNeto = request.MontoNeto;
        gasto.Moneda = request.Moneda;
        gasto.TipoGasto = request.TipoGasto;
        gasto.EsDeducibleRenta = request.EsDeducibleRenta;
        gasto.Descripcion = request.Descripcion;
        gasto.UpdatedAt = DateTimeOffset.UtcNow;

        string valorNuevo = JsonSerializer.Serialize(new 
        { 
            gasto.MontoTotal, gasto.Proveedor, gasto.NumeroFactura, gasto.EsCreditoFiscalValido
        });

        await repository.UpdateGastoAsync(gasto);

        await repository.AddAuditoriaAsync(new AuditoriaOperacion
        {
            UsuarioId = gasto.UsuarioId,
            Operacion = OperacionAuditoria.UPDATE,
            TablaAfectada = "gasto",
            RegistroId = gasto.GastoId,
            OldValues = valorAnterior,
            NewValues = valorNuevo,
            CamposModificados = ["proveedor", "numero_factura", "fecha_emision", "monto_total", "monto_iva_soportado", "monto_neto", "moneda", "tipo_gasto", "es_deducible_renta", "descripcion"],
            Justificacion = request.Justificacion
        });

        await repository.SaveChangesAsync();
        return (true, null);
    }

    // ========================================================================
    // Implementación de HU-007: Eliminación con Auditoría Estricta (Ley 8968)
    // ========================================================================
    public async Task<(bool success, string? error)> EliminarGastoAsync(Guid id, DeleteGastoRequest request)
    {
        // 1. Validación Estricta de la Justificación
        if (string.IsNullOrWhiteSpace(request.Justificacion))
        {
            return (false, "Error: La Ley 8968 exige proporcionar una Justificación válida para eliminar un comprobante.");
        }

        var gasto = await repository.GetGastoByIdAsync(id);
        if (gasto == null) return (false, "Gasto no encontrado.");

        string valorAnterior = JsonSerializer.Serialize(new 
        { 
            gasto.MontoTotal, gasto.Proveedor, gasto.NumeroFactura
        });

        await repository.DeleteGastoAsync(gasto);

        await repository.AddAuditoriaAsync(new AuditoriaOperacion
        {
            UsuarioId = gasto.UsuarioId,
            Operacion = OperacionAuditoria.DELETE,
            TablaAfectada = "gasto",
            RegistroId = gasto.GastoId,
            OldValues = valorAnterior,
            Justificacion = request.Justificacion
        });

        await repository.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool success, string? error)> ActualizarReservaAsync(string id, UpdateReservaRequest request)
    {
        string razon = request.ObtenerRazon();
        if (string.IsNullOrWhiteSpace(razon))
        {
            return (false, "Error: La Ley 8968 exige proporcionar una Razón de cambio o Justificación válida para modificar una reserva.");
        }

        if (Guid.TryParse(id, out var guidId))
        {
            var reserva = await repository.GetReservaByIdAsync(guidId);
            if (reserva != null)
            {
                string valorAnterior = JsonSerializer.Serialize(new
                {
                    reserva.MontoColones,
                    reserva.MontoBruto,
                    reserva.NombreHuesped,
                    reserva.FechaInicio
                });

                reserva.MontoColones = request.Monto;
                reserva.MontoBruto = request.Monto;
                if (request.FechaReserva.HasValue)
                {
                    reserva.FechaInicio = request.FechaReserva.Value;
                }
                if (!string.IsNullOrWhiteSpace(request.Huesped))
                {
                    reserva.NombreHuesped = request.Huesped;
                }

                string valorNuevo = JsonSerializer.Serialize(new
                {
                    reserva.MontoColones,
                    reserva.MontoBruto,
                    reserva.NombreHuesped,
                    reserva.FechaInicio
                });

                await repository.UpdateReservaAsync(reserva);
                await repository.AddAuditoriaAsync(new AuditoriaOperacion
                {
                    UsuarioId = reserva.UsuarioId,
                    Operacion = OperacionAuditoria.UPDATE,
                    TablaAfectada = "reserva",
                    RegistroId = reserva.ReservaId,
                    OldValues = valorAnterior,
                    NewValues = valorNuevo,
                    CamposModificados = ["monto_colones", "monto_bruto", "nombre_huesped", "fecha_inicio"],
                    Justificacion = razon
                });

                await repository.SaveChangesAsync();
                return (true, null);
            }
        }

        if (int.TryParse(id, out var intId))
        {
            var reservaDirecta = await repository.GetReservaDirectaByIdAsync(intId);
            if (reservaDirecta != null)
            {
                string valorAnterior = JsonSerializer.Serialize(new
                {
                    reservaDirecta.Monto,
                    reservaDirecta.Huesped,
                    reservaDirecta.FechaReserva
                });

                reservaDirecta.Monto = request.Monto;
                if (request.FechaReserva.HasValue)
                {
                    reservaDirecta.FechaReserva = request.FechaReserva.Value;
                }
                if (!string.IsNullOrWhiteSpace(request.Huesped))
                {
                    reservaDirecta.Huesped = request.Huesped;
                }

                string valorNuevo = JsonSerializer.Serialize(new
                {
                    reservaDirecta.Monto,
                    reservaDirecta.Huesped,
                    reservaDirecta.FechaReserva
                });

                await repository.UpdateReservaDirectaAsync(reservaDirecta);
                Guid.TryParse(reservaDirecta.AnfitrionId, out var usuarioId);

                await repository.AddAuditoriaAsync(new AuditoriaOperacion
                {
                    UsuarioId = usuarioId,
                    Operacion = OperacionAuditoria.UPDATE,
                    TablaAfectada = "reserva_directa",
                    RegistroId = Guid.Empty,
                    OldValues = valorAnterior,
                    NewValues = valorNuevo,
                    CamposModificados = ["monto", "huesped", "fecha_reserva"],
                    Justificacion = razon
                });

                await repository.SaveChangesAsync();
                return (true, null);
            }
        }

        return (false, "Reserva no encontrada.");
    }

    public async Task<(bool success, string? error)> EliminarReservaAsync(string id, DeleteReservaRequest request)
    {
        string razon = request.ObtenerRazon();
        if (string.IsNullOrWhiteSpace(razon))
        {
            return (false, "Error: La Ley 8968 exige proporcionar una Razón de cambio o Justificación válida para eliminar una reserva.");
        }

        if (Guid.TryParse(id, out var guidId))
        {
            var reserva = await repository.GetReservaByIdAsync(guidId);
            if (reserva != null)
            {
                string valorAnterior = JsonSerializer.Serialize(new
                {
                    reserva.MontoColones,
                    reserva.NombreHuesped,
                    reserva.FechaInicio
                });

                await repository.DeleteReservaAsync(reserva);
                await repository.AddAuditoriaAsync(new AuditoriaOperacion
                {
                    UsuarioId = reserva.UsuarioId,
                    Operacion = OperacionAuditoria.DELETE,
                    TablaAfectada = "reserva",
                    RegistroId = reserva.ReservaId,
                    OldValues = valorAnterior,
                    Justificacion = razon
                });

                await repository.SaveChangesAsync();
                return (true, null);
            }
        }

        if (int.TryParse(id, out var intId))
        {
            var reservaDirecta = await repository.GetReservaDirectaByIdAsync(intId);
            if (reservaDirecta != null)
            {
                string valorAnterior = JsonSerializer.Serialize(new
                {
                    reservaDirecta.Monto,
                    reservaDirecta.Huesped,
                    reservaDirecta.FechaReserva
                });

                await repository.DeleteReservaDirectaAsync(reservaDirecta);
                Guid.TryParse(reservaDirecta.AnfitrionId, out var usuarioId);

                await repository.AddAuditoriaAsync(new AuditoriaOperacion
                {
                    UsuarioId = usuarioId,
                    Operacion = OperacionAuditoria.DELETE,
                    TablaAfectada = "reserva_directa",
                    RegistroId = Guid.Empty,
                    OldValues = valorAnterior,
                    Justificacion = razon
                });

                await repository.SaveChangesAsync();
                return (true, null);
            }
        }

        return (false, "Reserva no encontrada.");
    }

    public async Task<IEnumerable<FiscalHost.Api.CR.Models.DTOs.Operations.Responses.GastoDto>> GetGastosAsync(Guid usuarioId)
    {
        var gastos = await repository.GetGastosByUsuarioIdAsync(usuarioId);
        return gastos.Select(g => new FiscalHost.Api.CR.Models.DTOs.Operations.Responses.GastoDto
        {
            GastoId = g.GastoId,
            PropiedadId = g.PropiedadId,
            Proveedor = g.Proveedor,
            Descripcion = g.Descripcion,
            NumeroFactura = g.NumeroFactura,
            FechaEmision = g.FechaEmision,
            MontoTotal = g.MontoTotal,
            EsCreditoFiscalValido = g.EsCreditoFiscalValido,
            EstadoOcr = g.EstadoOcr,
            EstadoValidacion = g.EstadoValidacion
        });
    }

    public async Task<IEnumerable<FiscalHost.Api.CR.Models.DTOs.Operations.Responses.ReservaDto>> GetReservasAsync(Guid usuarioId)
    {
        var reservas = await repository.GetReservasByUsuarioIdAsync(usuarioId);
        return reservas.Select(r => new FiscalHost.Api.CR.Models.DTOs.Operations.Responses.ReservaDto
        {
            ReservaId = r.ReservaId,
            FechaInicio = r.FechaInicio,
            MontoColones = r.MontoColones,
            NombreHuesped = r.NombreHuesped,
            ClasificacionFiscal = r.ClasificacionFiscal,
            PlataformaOrigen = r.PlataformaOrigen
        });
    }
}
