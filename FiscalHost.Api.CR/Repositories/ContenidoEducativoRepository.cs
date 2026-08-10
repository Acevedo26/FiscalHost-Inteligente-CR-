using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.Entities.Communication;

namespace FiscalHost.Api.CR.Repositories;

public class ContenidoEducativoRepository(AppDbContext dbContext) : IContenidoEducativoRepository
{
	public async Task<ContenidoEducativo?> GetBySlugAsync(string slug, bool soloPublicados)
	{
		var query = dbContext.ContenidosEducativos.Where(c => c.Slug == slug);

		if (soloPublicados)
		{
			query = query.Where(c => c.Publicado);
		}

		return await query.FirstOrDefaultAsync();
	}

	public async Task<ContenidoEducativo?> GetByIdAsync(Guid contenidoId)
	{
		return await dbContext.ContenidosEducativos
			.FirstOrDefaultAsync(c => c.ContenidoId == contenidoId);
	}

	public async Task<IEnumerable<string>> GetCategoriasDisponiblesAsync()
	{
		return await dbContext.ContenidosEducativos
			.Where(c => c.Publicado)
			.Select(c => c.Categoria)
			.Distinct()
			.OrderBy(c => c)
			.ToListAsync();
	}

	public async Task<IEnumerable<ContenidoEducativo>> GetByCategoriaAsync(string categoria, bool soloPublicados)
	{
		var query = dbContext.ContenidosEducativos.Where(c => c.Categoria == categoria);

		if (soloPublicados)
		{
			query = query.Where(c => c.Publicado);
		}

		return await query
			.OrderBy(c => c.OrdenDisplay)
			.ToListAsync();
	}

	public async Task<IEnumerable<ContenidoEducativo>> GetTutorialesPrimerUsoAsync(bool soloPublicados)
	{
		var query = dbContext.ContenidosEducativos.Where(c => c.EsTutorialPrimerUso);

		if (soloPublicados)
		{
			query = query.Where(c => c.Publicado);
		}

		return await query
			.OrderBy(c => c.OrdenDisplay)
			.ToListAsync();
	}

	public async Task<bool> ExisteSlugAsync(string slug)
	{
		return await dbContext.ContenidosEducativos.AnyAsync(c => c.Slug == slug);
	}

	public async Task AddAsync(ContenidoEducativo contenido)
	{
		await dbContext.ContenidosEducativos.AddAsync(contenido);
	}

	public Task UpdateAsync(ContenidoEducativo contenido)
	{
		dbContext.ContenidosEducativos.Update(contenido);
		return Task.CompletedTask;
	}

	public async Task SaveChangesAsync()
	{
		await dbContext.SaveChangesAsync();
	}
}