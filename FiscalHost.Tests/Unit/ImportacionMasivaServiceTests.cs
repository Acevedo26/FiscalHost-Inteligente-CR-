using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace FiscalHost.Tests.Unit;

public class ImportacionMasivaServiceTests
{
	[Fact]
	public async Task ImportarAsync_DebeLanzarExcepcion_CuandoArchivoEstaVacio()
	{
		var repositoryMock = new Mock<IImportacionMasivaRepository>();

		var service = new ImportacionMasivaService(
			repositoryMock.Object);

		var archivoMock = new Mock<IFormFile>();

		archivoMock.Setup(x => x.Length).Returns(0);
		archivoMock.Setup(x => x.FileName).Returns("reservas.csv");

		var usuarioId = Guid.NewGuid();

		var ex = await Assert.ThrowsAsync<Exception>(() =>
			service.ImportarAsync(
				archivoMock.Object,
				usuarioId));

		Assert.Equal(
			"El archivo está vacío o no fue enviado.",
			ex.Message);
	}

	[Fact]
	public async Task ImportarAsync_DebeLanzarExcepcion_CuandoArchivoNoEsCsv()
	{
		var repositoryMock = new Mock<IImportacionMasivaRepository>();

		var service = new ImportacionMasivaService(
			repositoryMock.Object);

		var archivoMock = new Mock<IFormFile>();

		archivoMock.Setup(x => x.Length).Returns(100);
		archivoMock.Setup(x => x.FileName).Returns("archivo.txt");

		var usuarioId = Guid.NewGuid();

		var ex = await Assert.ThrowsAsync<Exception>(() =>
			service.ImportarAsync(
				archivoMock.Object,
				usuarioId));

		Assert.Equal(
			"Solo se permiten archivos CSV.",
			ex.Message);
	}

	[Fact]
	public void GenerarPlantillaCsv_DebeRetornarPlantillaCorrecta()
	{
		var repositoryMock = new Mock<IImportacionMasivaRepository>();

		var service = new ImportacionMasivaService(
			repositoryMock.Object);

		var resultado = service.GenerarPlantillaCsv();

		Assert.Contains("FechaInicio", resultado);
		Assert.Contains("FechaFin", resultado);
		Assert.Contains("MontoBruto", resultado);
		Assert.Contains("PlataformaOrigen", resultado);
		Assert.Contains("ReferenciaPlataforma", resultado);
	}

	[Fact]
	public async Task ObtenerReporteErroresCsvAsync_DebeLanzarExcepcion_SiNoExisteImportacion()
	{
		var repositoryMock = new Mock<IImportacionMasivaRepository>();

		repositoryMock
			.Setup(x => x.GetImportacionAsync(It.IsAny<Guid>()))
			.ReturnsAsync((ImportacionMasiva?)null);

		var service = new ImportacionMasivaService(
			repositoryMock.Object);

		var ex = await Assert.ThrowsAsync<Exception>(() =>
			service.ObtenerReporteErroresCsvAsync(Guid.NewGuid()));

		Assert.Equal(
			"No se encontró la importación.",
			ex.Message);
	}

	[Fact]
	public async Task ObtenerReporteErroresCsvAsync_DebeGenerarCsvCorrectamente()
	{
		var repositoryMock = new Mock<IImportacionMasivaRepository>();

		var importacion = new ImportacionMasiva
		{
			DetalleErrores =
				"[\"Fila 2: PlataformaOrigen inválida.\"]"
		};

		repositoryMock
			.Setup(x => x.GetImportacionAsync(It.IsAny<Guid>()))
			.ReturnsAsync(importacion);

		var service = new ImportacionMasivaService(
			repositoryMock.Object);

		var resultado =
			await service.ObtenerReporteErroresCsvAsync(Guid.NewGuid());

		Assert.Contains("Error", resultado);
		Assert.Contains(
			"Fila 2: PlataformaOrigen inválida.",
			resultado);
	}
}