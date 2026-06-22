using System;
using System.Text.Json;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.DTOs.Operations.Requests;
using FiscalHost.Api.CR.Models.Entities.Audit;
using FiscalHost.Api.CR.Models.Entities.Operations;
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
            Entidad = "ReservaDirecta",
            Usuario = request.AnfitrionId.ToString(),
            Accion = "CREACION",
            Descripcion = "Reserva directa registrada."
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
            Entidad = "Gasto",
            Usuario = request.UsuarioId.ToString(),
            Accion = "CREACION",
            Descripcion = "Gasto registrado."
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
            // 1. Almacenamiento Seguro (AES-256 manejado por Azure Blob Storage)
            using var stream = request.Comprobante.OpenReadStream();
            string blobUrl = await blobStorageService.UploadAsync(
                stream, 
                request.Comprobante.FileName, 
                request.Comprobante.ContentType);

            // 2. Extracción de Metadatos mediante OCR
            stream.Position = 0; // Reiniciar stream para lectura del OCR
            var ocrResult = await ocrService.ExtractMetadataAsync(stream, request.Comprobante.ContentType);

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

            var gasto = new Gasto
            {
                UsuarioId = request.UsuarioId,
                PropiedadId = request.PropiedadId,
                Proveedor = ocrResult.Proveedor ?? "PENDIENTE",
                NumeroFactura = ocrResult.NumeroFactura ?? "PENDIENTE",
                FechaEmision = ocrResult.FechaEmision ?? DateOnly.FromDateTime(DateTime.UtcNow),
                MontoTotal = ocrResult.MontoTotal ?? 0,
                EvidenciaUrl = blobUrl,
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
                Entidad = "Gasto",
                Usuario = request.UsuarioId.ToString(),
                Accion = "CREACION",
                Descripcion = ocrResult.Exitoso 
                    ? "Gasto subido y procesado exitosamente por OCR."
                    : "Gasto subido con Fallo de OCR. Se requiere revisión manual.",
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

        // 2. Registro Obligatorio en la Bitácora con Justificación
        await repository.AddAuditoriaAsync(new AuditoriaOperacion
        {
            Entidad = "Gasto",
            EntidadId = gasto.GastoId.GetHashCode(),
            Usuario = gasto.UsuarioId.ToString(),
            Accion = "UPDATE",
            Descripcion = "Modificación de gasto validado.",
            ValorAnterior = valorAnterior,
            ValorNuevo = valorNuevo,
            Justificacion = request.Justificacion // <- Crucial para auditoría
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

        // 2. Registro Obligatorio en la Bitácora
        await repository.AddAuditoriaAsync(new AuditoriaOperacion
        {
            Entidad = "Gasto",
            EntidadId = gasto.GastoId.GetHashCode(),
            Usuario = gasto.UsuarioId.ToString(),
            Accion = "DELETE",
            Descripcion = "Eliminación de gasto validado.",
            ValorAnterior = valorAnterior,
            ValorNuevo = "null",
            Justificacion = request.Justificacion // <- Crucial para auditoría
        });

        await repository.SaveChangesAsync();
        return (true, null);
    }
}
