using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums;

namespace FiscalHost.Api.CR.Repositories;

public interface IObligacionTributariaRepository
{
    Task<ObligacionTributaria?> GetByIdAsync(Guid id);
    Task<IEnumerable<ObligacionTributaria>> GetVencidasPendientesAsync(DateOnly fechaCorte);
    Task UpdateAsync(ObligacionTributaria obligacion);
    Task UpdateRangeAsync(IEnumerable<ObligacionTributaria> obligaciones);
    Task SaveChangesAsync();
}
