using System;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;

namespace FiscalHost.Api.CR.Services;

public interface IObligacionTributariaService
{
    Task<ObligacionTributariaDto?> ConsultarDeudaAsync(Guid obligacionId);
    Task ProcesarCargosMoratoriosMasivosAsync(DateOnly fechaCorte);
    Task CalcularInteresesObligacionAsync(ObligacionTributaria obligacion, DateOnly fechaActual, bool notificarCambios);
}
