using FiscalHost.Api.CR.Models.DTOs;
using FiscalHost.Api.CR.Models.Entities;
using FiscalHost.Api.CR.Models.Enums;
using FiscalHost.Api.CR.Repositories;

namespace FiscalHost.Api.CR.Services;

public interface IConfiguracionTributariaService
{
    Task<IEnumerable<ActividadEconomicaResponse>> GetActividadesAsync();
    Task<(bool success, string? error, ConfiguracionTributariaResponse? data)> GuardarConfiguracionAsync(ConfiguracionTributariaRequest request);
    Task<ConfiguracionTributariaResponse?> GetConfiguracionAsync(string anfitrionId);
}

public class ConfiguracionTributariaService(
    IConfiguracionTributariaRepository configRepo,
    IActividadEconomicaRepository actividadRepo) : IConfiguracionTributariaService
{
    private const string NisePattern = @"^\d{10}$"; // NISE: 10 dígitos

    public async Task<IEnumerable<ActividadEconomicaResponse>> GetActividadesAsync()
    {
        var actividades = await actividadRepo.GetAllActivasAsync();
        return actividades.Select(a => new ActividadEconomicaResponse
        {
            Id = a.Id,
            Codigo = a.Codigo,
            Descripcion = a.Descripcion
        });
    }

    public async Task<(bool success, string? error, ConfiguracionTributariaResponse? data)> GuardarConfiguracionAsync(
        ConfiguracionTributariaRequest request)
    {
        // Validar código de actividad económica contra catálogo DGT
        var actividad = await actividadRepo.GetByCodigoAsync(request.CodigoActividad);
        if (actividad is null)
            return (false, $"El código de actividad económica '{request.CodigoActividad}' no existe en el catálogo DGT.", null);

        // Validar NISE (10 dígitos numéricos)
        if (!System.Text.RegularExpressions.Regex.IsMatch(request.Nise, NisePattern))
            return (false, "El NISE debe contener exactamente 10 dígitos numéricos.", null);

        var existing = await configRepo.GetByAnfitrionIdAsync(request.AnfitrionId);
        string? advertencia = null;

        if (existing is null)
        {
            var config = new ConfiguracionTributaria
            {
                AnfitrionId = request.AnfitrionId,
                ActividadEconomicaId = actividad.Id,
                TribuCr = GenerarTribuCr(request.AnfitrionId, request.CodigoActividad),
                DireccionFiscal = request.DireccionFiscal,
                Nise = request.Nise
            };
            await configRepo.AddAsync(config);
            await configRepo.SaveChangesAsync();

            await RegistrarAuditoriaAsync(config.Id, "CREACION", string.Empty, request.CodigoActividad,
                "Configuración tributaria creada.");
            existing = config;
            existing.ActividadEconomica = actividad;
        }
        else
        {
            var codigoAnterior = existing.ActividadEconomica?.Codigo ?? string.Empty;
            bool cambioActividad = codigoAnterior != request.CodigoActividad;

            if (cambioActividad)
                advertencia = "El cambio de actividad económica puede afectar sus obligaciones fiscales vigentes. Consulte con un contador autorizado.";

            existing.ActividadEconomicaId = actividad.Id;
            existing.TribuCr = GenerarTribuCr(request.AnfitrionId, request.CodigoActividad);
            existing.DireccionFiscal = request.DireccionFiscal;
            existing.Nise = request.Nise;
            existing.FechaActualizacion = DateTime.UtcNow;
            existing.ActividadEconomica = actividad;

            await configRepo.SaveChangesAsync();

            if (cambioActividad)
                await RegistrarAuditoriaAsync(existing.Id, "CAMBIO_ACTIVIDAD", codigoAnterior, request.CodigoActividad,
                    "Cambio de actividad económica. Obligaciones fiscales recalculadas.");
        }

        return (true, null, MapToResponse(existing, advertencia));
    }

    public async Task<ConfiguracionTributariaResponse?> GetConfiguracionAsync(string anfitrionId)
    {
        var config = await configRepo.GetByAnfitrionIdAsync(anfitrionId);
        return config is null ? null : MapToResponse(config, null);
    }

    private async Task RegistrarAuditoriaAsync(int configId, string campo, string anterior, string nuevo, string descripcion)
    {
        await configRepo.AddAuditoriaAsync(new AuditoriaConfiguracion
        {
            ConfiguracionTributariaId = configId,
            Campo = campo,
            ValorAnterior = anterior,
            ValorNuevo = nuevo,
            Descripcion = descripcion
        });
        await configRepo.SaveChangesAsync();
    }

    // Genera identificador TRIBU-CR: prefijo fijo + anfitrionId + código actividad
    private static string GenerarTribuCr(string anfitrionId, string codigoActividad) =>
        $"TRIBU-{anfitrionId.ToUpper()[..Math.Min(6, anfitrionId.Length)]}-{codigoActividad}";

    private static ConfiguracionTributariaResponse MapToResponse(ConfiguracionTributaria c, string? advertencia) => new()
    {
        Id = c.Id,
        AnfitrionId = c.AnfitrionId,
        CodigoActividad = c.ActividadEconomica?.Codigo ?? string.Empty,
        DescripcionActividad = c.ActividadEconomica?.Descripcion ?? string.Empty,
        TribuCr = c.TribuCr,
        DireccionFiscal = c.DireccionFiscal,
        Nise = c.Nise,
        Estado = "Activa",
        FechaActualizacion = c.FechaActualizacion,
        Advertencia = advertencia
    };
}
