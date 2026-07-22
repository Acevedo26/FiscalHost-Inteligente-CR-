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
        await repository.AddAuditoriaAsync(new AuditoriaOperacion
        {
            Operacion = OperacionAuditoria.INSERT,
            TablaAfectada = "reserva_directa",
            NewValues = JsonSerializer.Serialize(reserva)
        });
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
        await repository.AddAuditoriaAsync(new AuditoriaOperacion
        {
            UsuarioId = request.UsuarioId,
            Operacion = OperacionAuditoria.INSERT,
            TablaAfectada = "gasto",
            NewValues = JsonSerializer.Serialize(new { gasto.Proveedor, gasto.NumeroFactura, gasto.MontoTotal })
        });
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
                return (false, JsonSerializer.Serialize(new
                {
                    mensaje = "El OCR no pudo extraer los datos del comprobante. Complete los campos manualmente.",
                    exitoOcr = false,
                    evidenciaUrl = fileUrl,
                    datosExtraidos = ocrResult
                }));
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
}
