using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.Identity;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Repositories;

public interface ICalculoRentaCapitalRepository
{
	Task<PerfilTributario?> GetPerfilTributarioAsync(Guid usuarioId);

	Task<PeriodoFiscal?> GetPeriodoFiscalAsync(short anio, short mes, TipoFormulario tipoFormulario);

	Task<List<Reserva>> GetReservasDelPeriodoAsync(Guid usuarioId, short anio, short mes);

	Task<List<Gasto>> GetGastosDeduciblesDelPeriodoAsync(Guid usuarioId, short anio, short mes);

	Task<CalculoFiscal?> GetCalculoExistenteAsync(Guid usuarioId, Guid periodoId, TipoFormulario tipoFormulario);

	Task AddCalculoAsync(CalculoFiscal calculo);

	Task AddAuditoriaCambioRegimenAsync(AuditoriaOperacion auditoria);

	Task SaveChangesAsync();
}

public class CalculoRentaCapitalRepository(AppDbContext context) : ICalculoRentaCapitalRepository
{
	public Task<PerfilTributario?> GetPerfilTributarioAsync(Guid usuarioId) =>
		context.PerfilesTributarios
			.FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);

	public Task<PeriodoFiscal?> GetPeriodoFiscalAsync(short anio, short mes, TipoFormulario tipoFormulario) =>
		context.PeriodosFiscales
			.FirstOrDefaultAsync(p => p.Anio == anio && p.Mes == mes && p.TipoFormulario == tipoFormulario);

	public Task<List<Reserva>> GetReservasDelPeriodoAsync(Guid usuarioId, short anio, short mes) =>
		context.Reservas
			.Where(r => r.UsuarioId == usuarioId
						&& r.PeriodoFiscalAnio == anio
						&& r.PeriodoFiscalMes == mes)
			.ToListAsync();

	public Task<List<Gasto>> GetGastosDeduciblesDelPeriodoAsync(Guid usuarioId, short anio, short mes) =>
		context.Gastos
			.Where(g => g.UsuarioId == usuarioId
						&& g.PeriodoFiscalAnio == anio
						&& g.PeriodoFiscalMes == mes
						&& g.EsDeducibleRenta
						&& g.DeletedAt == null)
			.ToListAsync();

	public Task<CalculoFiscal?> GetCalculoExistenteAsync(Guid usuarioId, Guid periodoId, TipoFormulario tipoFormulario) =>
		context.CalculosFiscales
			.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId
									  && c.PeriodoId == periodoId
									  && c.TipoFormulario == tipoFormulario);

	public async Task AddCalculoAsync(CalculoFiscal calculo) =>
		await context.CalculosFiscales.AddAsync(calculo);

	public async Task AddAuditoriaCambioRegimenAsync(AuditoriaOperacion auditoria) =>
		await context.AuditoriasOperacion.AddAsync(auditoria);

	public Task SaveChangesAsync() => context.SaveChangesAsync();
}
