using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FiscalHost.Tests.Integration;

public class ImportacionMasivaIntegrationTests : IDisposable
{
	private readonly AppDbContext _db;
	private readonly ImportacionMasivaService _service;

	public ImportacionMasivaIntegrationTests()
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;

		_db = new AppDbContext(options);

		var repo = new ImportacionMasivaRepository(_db);

		_service = new ImportacionMasivaService(repo);
	}

	[Fact]
	public async Task ImportarArchivoVacio_NoGuardaImportacion()
	{
		var archivo = CrearFormFile("reservas.csv", []);

		await Assert.ThrowsAsync<Exception>(() =>
			_service.ImportarAsync(
				archivo,
				Guid.NewGuid()));

		Assert.Empty(_db.ImportacionesMasivas);
	}

	public void Dispose() => _db.Dispose();

	private static IFormFile CrearFormFile(string nombre, byte[] contenido)
	{
		var stream = new MemoryStream(contenido);

		return new FormFile(
			stream,
			0,
			contenido.Length,
			"Archivo",
			nombre);
	}
}