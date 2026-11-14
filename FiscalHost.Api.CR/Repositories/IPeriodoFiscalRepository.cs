using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums.TaxIntelligence;

namespace FiscalHost.Api.CR.Repositories;

/// <summary>
/// Interfaz para el repositorio de períodos fiscales.
/// </summary>
public interface IPeriodoFiscalRepository
{
    Task<PeriodoFiscal?> GetByAnioMesTipoAsync(short anio, short mes, TipoFormulario tipoFormulario);
    Task<List<PeriodoFiscal>> GetByAnioRangoTipoAsync(short anioInicio, short anioFin, TipoFormulario tipoFormulario);
    Task<bool> ExisteAsync(short anio, short mes, TipoFormulario tipoFormulario);
}