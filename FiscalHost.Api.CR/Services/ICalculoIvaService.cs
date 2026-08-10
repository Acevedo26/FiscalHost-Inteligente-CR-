using System;
using System.Threading.Tasks;

namespace FiscalHost.Api.CR.Services;

public interface ICalculoIvaService
{
    Task<CalculoIvaResponseDto> CalcularIvaDevengadoAsync(Guid usuarioId, short anio, short mes);
}
