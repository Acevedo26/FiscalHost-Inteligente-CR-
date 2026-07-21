using System;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence;

namespace FiscalHost.Api.CR.Services;

public interface ICalculoIvaService
{
    Task<CalculoIvaResponseDto> CalcularIvaDevengadoAsync(Guid usuarioId, short anio, short mes);
}
