using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.Identity;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Repositories;

public interface ISancionAutoliquidacionRepository
{
	Task<ObligacionTributaria?> GetObligacionPendienteAsync(Guid usuarioId, Guid obligacionId);

	Task<PerfilTributario?> GetPerfilTributarioAsync(Guid usuarioId);

	Task<PeriodoFiscal?> GetPeriodoConSalarioBaseAsync(short anio, TipoFormulario tipoFormulario);

	Task<SancionAutoliquidacion?> GetSancionExistenteAsync(Guid obligacionId);

	Task AddSancionAsync(SancionAutoliquidacion sancion);

	Task SaveChangesAsync();
}

public class SancionAutoliquidacionRepository(AppDbContext context) : ISancionAutoliquidacionRepository
{
	public Task<ObligacionTributaria?> GetObligacionPendienteAsync(Guid usuarioId, Guid obligacionId) =>
		context.ObligacionesTributarias
			.FirstOrDefaultAsync(o => o.UsuarioId == usuarioId
									  && o.ObligacionId == obligacionId
									  && o.Estado != EstadoObligacion.PAGADA);

	public Task<PerfilTributario?> GetPerfilTributarioAsync(Guid usuarioId) =>
		context.PerfilesTributarios
			.FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);

	public Task<PeriodoFiscal?> GetPeriodoConSalarioBaseAsync(short anio, TipoFormulario tipoFormulario) =>
		context.PeriodosFiscales
			.Where(p => p.Anio == anio && p.TipoFormulario == tipoFormulario && p.SalarioBaseVigente != null)
			.OrderByDescending(p => p.Mes)
			.FirstOrDefaultAsync();

	public Task<SancionAutoliquidacion?> GetSancionExistenteAsync(Guid obligacionId) =>
		context.SancionesAutoliquidacion
			.FirstOrDefaultAsync(s => s.ObligacionId == obligacionId);

	public async Task AddSancionAsync(SancionAutoliquidacion sancion) =>
		await context.SancionesAutoliquidacion.AddAsync(sancion);

	public Task SaveChangesAsync() => context.SaveChangesAsync();
}
