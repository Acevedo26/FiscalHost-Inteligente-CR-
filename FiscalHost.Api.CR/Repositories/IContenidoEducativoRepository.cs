using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Models.Entities.Communication;

namespace FiscalHost.Api.CR.Repositories;

public interface IContenidoEducativoRepository
{
	Task<ContenidoEducativo?> GetBySlugAsync(string slug, bool soloPublicados);
	Task<ContenidoEducativo?> GetByIdAsync(Guid contenidoId);
	Task<IEnumerable<ContenidoEducativo>> GetByCategoriaAsync(string categoria, bool soloPublicados);
	Task<IEnumerable<ContenidoEducativo>> GetTutorialesPrimerUsoAsync(bool soloPublicados);
	Task<IEnumerable<string>> GetCategoriasDisponiblesAsync();
	Task<bool> ExisteSlugAsync(string slug);
	Task AddAsync(ContenidoEducativo contenido);
	Task UpdateAsync(ContenidoEducativo contenido);
	Task SaveChangesAsync();
}