using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using FiscalHost.Api.CR.Models;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public class ObligacionTributariaService(
    IObligacionTributariaRepository repository,
    IOptions<TaxSettings> taxSettings,
    INotificacionService notificacionService,
    ILogger<ObligacionTributariaService> logger) : IObligacionTributariaService
{
    public async Task<ObligacionTributariaDto?> ConsultarDeudaAsync(Guid obligacionId)
    {
        var obligacion = await repository.GetByIdAsync(obligacionId);
        if (obligacion == null) return null;

        return new ObligacionTributariaDto
        {
            ObligacionId = obligacion.ObligacionId,
            UsuarioId = obligacion.UsuarioId,
            CalculoId = obligacion.CalculoId,
            PeriodoId = obligacion.PeriodoId,
            TipoFormulario = obligacion.TipoFormulario,
            Descripcion = obligacion.Descripcion,
            MontoCapital = obligacion.MontoCapital,
            MontoMulta = obligacion.MontoMulta,
            MontoInteresesAcumulados = obligacion.MontoInteresesAcumulados,
            MontoTotalActualizado = obligacion.MontoTotalActualizado,
            FechaVencimiento = new DateTime(obligacion.FechaVencimiento.Year, obligacion.FechaVencimiento.Month, obligacion.FechaVencimiento.Day),
            FechaPago = obligacion.FechaPago.HasValue ? new DateTime(obligacion.FechaPago.Value.Year, obligacion.FechaPago.Value.Month, obligacion.FechaPago.Value.Day) : null,
            Estado = obligacion.Estado,
            TasaInteresAplicada = obligacion.TasaInteresAplicada,
            HistorialIntereses = obligacion.HistorialIntereses
        };
    }

    public async Task ProcesarCargosMoratoriosMasivosAsync(DateOnly fechaCorte)
    {
        logger.LogInformation("Iniciando procesamiento masivo de mora para la fecha de corte: {FechaCorte}", fechaCorte);
        var pendientes = await repository.GetVencidasPendientesAsync(fechaCorte);
        
        foreach (var obligacion in pendientes)
        {
            await CalcularInteresesObligacionAsync(obligacion, fechaCorte, notificarCambios: true);
        }
        
        await repository.UpdateRangeAsync(pendientes);
        await repository.SaveChangesAsync();
        logger.LogInformation("Procesamiento masivo de mora finalizado.");
    }

    public async Task CalcularInteresesObligacionAsync(ObligacionTributaria obligacion, DateOnly fechaActual, bool notificarCambios)
    {
        if (obligacion.FechaVencimiento == default)
        {
            logger.LogWarning("Obligación {Id} no tiene FechaVencimiento válida. Se omite el cálculo de mora.", obligacion.ObligacionId);
            return;
        }

        if (obligacion.FechaVencimiento >= fechaActual)
        {
            // Aún no vence, no hay mora para el día actual o en el futuro.
            return;
        }

        // Revisar si ya se calculó para esta fecha para no duplicar si se ejecuta varias veces
        var fechaUltimoCalculo = obligacion.FechaUltimoCalculoInteres?.Date;
        if (fechaUltimoCalculo.HasValue && fechaUltimoCalculo.Value >= new DateTime(fechaActual.Year, fechaActual.Month, fechaActual.Day))
        {
            return; // Ya se calculó la mora hasta esta fecha
        }

        decimal tasaAnual = obligacion.TasaInteresAplicada ?? taxSettings.Value.DefaultInterestRate;
        int diasEnAno = DateTime.IsLeapYear(fechaActual.Year) ? 366 : 365;
        decimal tasaDiaria = tasaAnual / diasEnAno;

        // Base sobre la que se aplica interés (Capital + Multa)
        decimal baseCalculo = obligacion.MontoCapital + obligacion.MontoMulta;
        decimal moraDiaria = Math.Round(baseCalculo * tasaDiaria, 2);

        obligacion.MontoInteresesAcumulados += moraDiaria;
        obligacion.MontoTotalActualizado = obligacion.MontoCapital + obligacion.MontoMulta + obligacion.MontoInteresesAcumulados;
        obligacion.TasaInteresAplicada = tasaAnual;
        obligacion.FechaUltimoCalculoInteres = new DateTimeOffset(fechaActual.Year, fechaActual.Month, fechaActual.Day, 0, 0, 0, TimeSpan.Zero);

        // Actualizar historial
        // Deserializar JSON actual, añadir entrada, volver a serializar
        var dict = JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, decimal>>(obligacion.HistorialIntereses) ?? new();
        dict[fechaActual.ToString("yyyy-MM-dd")] = moraDiaria;
        obligacion.HistorialIntereses = JsonSerializer.Serialize(dict);

        if (notificarCambios && obligacion.UsuarioId != Guid.Empty)
        {
            await notificacionService.NotificarAsync(
                obligacion.UsuarioId.ToString(), 
                $"Tu obligación '{obligacion.Descripcion}' ha generado nuevos cargos por mora de ₡{moraDiaria}. Total actualizado: ₡{obligacion.MontoTotalActualizado}."
            );
        }
    }
}
