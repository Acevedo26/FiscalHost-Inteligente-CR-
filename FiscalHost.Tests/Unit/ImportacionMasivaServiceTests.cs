using CsvHelper;
using FiscalHost.Api.CR.Models.DTOs.Operations.Requests;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Globalization;
using System.IO;
using System.Text;
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

	[Fact]
	public async Task ImportarAsync_DebeImportarFilasValidasYReportarInvalidas_CuandoCsvTieneFechasMixtas()
	{
		// Arrange: CSV con 10 filas - 8 válidas, 2 con fechas inválidas
		var csvContent = new StringBuilder();
		csvContent.AppendLine("FechaInicio,FechaFin,MontoBruto,PlataformaOrigen,ReferenciaPlataforma");
		
		// 8 filas válidas
		for (int i = 1; i <= 8; i++)
		{
			csvContent.AppendLine($"2026-06-{i:D2},2026-06-{(i+2):D2},250000,AIRBNB,AIR-{i:D3}");
		}
		
		// 2 filas con fechas inválidas
		csvContent.AppendLine("31/02/2026,2026-06-15,250000,AIRBNB,AIR-009");  // 31/02 no existe
		csvContent.AppendLine("2026-13-01,2026-06-20,250000,AIRBNB,AIR-010");  // mes 13 no existe

		var archivoMock = new Mock<IFormFile>();
		var contentBytes = Encoding.UTF8.GetBytes(csvContent.ToString());
		var ms = new MemoryStream(contentBytes);
		
		archivoMock.Setup(x => x.Length).Returns(contentBytes.Length);
		archivoMock.Setup(x => x.FileName).Returns("fechas_mixtas.csv");
		archivoMock.Setup(x => x.OpenReadStream()).Returns(() => new MemoryStream(contentBytes));
		archivoMock.Setup(x => x.ContentType).Returns("text/csv");

		var repositoryMock = new Mock<IImportacionMasivaRepository>();
		ImportacionMasiva? capturedImportacion = null;
		List<Reserva>? capturedReservas = null;

		repositoryMock.Setup(x => x.AddImportacionAsync(It.IsAny<ImportacionMasiva>()))
			.Callback<ImportacionMasiva>(i => capturedImportacion = i)
			.Returns(Task.CompletedTask);
		
		repositoryMock.Setup(x => x.AddReservasAsync(It.IsAny<List<Reserva>>()))
			.Callback<List<Reserva>>(r => capturedReservas = r)
			.Returns(Task.CompletedTask);
		
		repositoryMock.Setup(x => x.ExisteReferenciaAsync(It.IsAny<Guid>(), It.IsAny<string>()))
			.ReturnsAsync(false);
		
		repositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

		var service = new ImportacionMasivaService(repositoryMock.Object);
		var usuarioId = Guid.NewGuid();

		// Act
		var resultado = await service.ImportarAsync(archivoMock.Object, usuarioId);

		// Assert
		Assert.True((bool)resultado.GetType().GetProperty("Success")!.GetValue(resultado)!);
		Assert.Equal(10, (int)resultado.GetType().GetProperty("TotalRegistros")!.GetValue(resultado)!);
		Assert.Equal(8, (int)resultado.GetType().GetProperty("RegistrosExitosos")!.GetValue(resultado)!);
		Assert.Equal(2, (int)resultado.GetType().GetProperty("RegistrosConError")!.GetValue(resultado)!);
		
		Assert.NotNull(capturedImportacion);
		Assert.Equal(EstadoImportacion.COMPLETADO_PARCIAL, capturedImportacion.Estado);
		
		Assert.NotNull(capturedReservas);
		Assert.Equal(8, capturedReservas.Count);
		
		// Verificar que el reporte de errores existe
		var reporteUrl = (string?)resultado.GetType().GetProperty("ReporteErroresUrl")!.GetValue(resultado);
		Assert.NotNull(reporteUrl);
		Assert.Contains("errores", reporteUrl!);
	}
}