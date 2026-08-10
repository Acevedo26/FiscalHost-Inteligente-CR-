using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.Entities.Communication;
using FiscalHost.Api.CR.Models.Enums.Communication;

namespace FiscalHost.Api.CR.Repositories;

public interface IAlertaRepository
{
	Task<Alerta?> GetByIdAsync(Guid alertaId);
	Task<bool> ExisteAlertaParaObligacionAsync(Guid obligacionId, TipoAlerta tipoAlerta);
	Task<IEnumerable<Alerta>> GetPendientesParaEnvioAsync(DateTimeOffset fechaCorte);
	Task<IEnumerable<Alerta>> GetByUsuarioAsync(Guid usuarioId, bool soloNoLeidas);
	Task AddAsync(Alerta alerta);
	Task AddRangeAsync(IEnumerable<Alerta> alertas);
	Task UpdateAsync(Alerta alerta);
	Task SaveChangesAsync();
}