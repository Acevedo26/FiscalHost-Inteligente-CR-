using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.Communication;
using FiscalHost.Api.CR.Models.Enums.Communication;

namespace FiscalHost.Api.CR.Repositories;

public class AlertaRepository(AppDbContext dbContext) : IAlertaRepository
{
	public async Task<Alerta?> GetByIdAsync(Guid alertaId)
	{
		return await dbContext.Alertas
			.Include(a => a.Usuario)
			.FirstOrDefaultAsync(a => a.AlertaId == alertaId);
	}

	public async Task<bool> ExisteAlertaParaObligacionAsync(Guid obligacionId, TipoAlerta tipoAlerta)
	{
		return await dbContext.Alertas
			.AnyAsync(a => a.ObligacionId == obligacionId && a.TipoAlerta == tipoAlerta);
	}

	public async Task<IEnumerable<Alerta>> GetPendientesParaEnvioAsync(DateTimeOffset fechaCorte)
	{
		return await dbContext.Alertas
			.Include(a => a.Usuario)
			.Where(a => a.Estado == EstadoNotificacion.PENDIENTE && a.FechaProgramada <= fechaCorte)
			.OrderBy(a => a.Prioridad)
			.ToListAsync();
	}

	public async Task<IEnumerable<Alerta>> GetByUsuarioAsync(Guid usuarioId, bool soloNoLeidas)
	{
		var query = dbContext.Alertas.Where(a => a.UsuarioId == usuarioId);

		if (soloNoLeidas)
		{
			query = query.Where(a => a.FechaLectura == null);
		}

		return await query
			.OrderByDescending(a => a.FechaProgramada)
			.ToListAsync();
	}

	public async Task AddAsync(Alerta alerta)
	{
		await dbContext.Alertas.AddAsync(alerta);
	}

	public async Task AddRangeAsync(IEnumerable<Alerta> alertas)
	{
		await dbContext.Alertas.AddRangeAsync(alertas);
	}

	public Task UpdateAsync(Alerta alerta)
	{
		dbContext.Alertas.Update(alerta);
		return Task.CompletedTask;
	}

	public async Task SaveChangesAsync()
	{
		await dbContext.SaveChangesAsync();
	}
}