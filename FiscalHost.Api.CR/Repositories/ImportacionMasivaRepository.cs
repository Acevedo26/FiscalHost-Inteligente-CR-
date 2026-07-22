using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.Operations;
using Microsoft.EntityFrameworkCore;

namespace FiscalHost.Api.CR.Repositories;

public interface IImportacionMasivaRepository
{
	Task AddImportacionAsync(ImportacionMasiva importacion);
	Task AddReservasAsync(List<Reserva> reservas);
	Task<bool> ExisteReferenciaAsync(Guid usuarioId, string referencia);
	Task<ImportacionMasiva?> GetImportacionAsync(Guid importacionId);
	Task SaveChangesAsync();
}

public class ImportacionMasivaRepository(AppDbContext db)
	: IImportacionMasivaRepository
{
	public async Task AddImportacionAsync(ImportacionMasiva importacion)
	{
		await db.ImportacionesMasivas.AddAsync(importacion);
	}

	public async Task AddReservasAsync(List<Reserva> reservas)
	{
		await db.Reservas.AddRangeAsync(reservas);
	}

	public Task<bool> ExisteReferenciaAsync(Guid usuarioId, string referencia)
	{
		return db.Reservas.AnyAsync(r =>
			r.UsuarioId == usuarioId &&
			r.ReferenciaPlataforma == referencia);
	}

	public Task<ImportacionMasiva?> GetImportacionAsync(Guid importacionId)
	{
		return db.ImportacionesMasivas
			.FirstOrDefaultAsync(i => i.ImportacionId == importacionId);
	}

	public Task SaveChangesAsync()
	{
		return db.SaveChangesAsync();
	}
}