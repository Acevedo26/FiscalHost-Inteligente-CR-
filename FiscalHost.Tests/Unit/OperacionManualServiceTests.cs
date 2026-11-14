using FiscalHost.Api.CR.Exceptions;
using FiscalHost.Api.CR.Models.DTOs;
using FiscalHost.Api.CR.Models.DTOs.Operations.Requests;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using NSubstitute;

namespace FiscalHost.Tests;

public class OperacionManualServiceTests
{
    private readonly IOperacionManualRepository _repository =
        Substitute.For<IOperacionManualRepository>();
    private readonly IBlobStorageService _blobStorageService = 
        Substitute.For<IBlobStorageService>();
    private readonly IOcrService _ocrService = 
        Substitute.For<IOcrService>();

    private readonly OperacionManualService _sut;

    public OperacionManualServiceTests()
    {
        _sut = new OperacionManualService(_repository, _blobStorageService, _ocrService);
    }

    [Fact]
    public async Task RegistrarReserva_MontoInvalido_RetornaError()
    {
        var request = new ReservaDirectaRequest
        {
            AnfitrionId = "anf-001",
            Huesped = "Juan Perez",
            FechaReserva = DateTime.UtcNow,
            Monto = 0
        };

        var (success, error) =
            await _sut.RegistrarReservaAsync(request);

        Assert.False(success);
        Assert.Contains("monto", error!,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RegistrarReserva_FechaFutura_RetornaError()
    {
        var request = new ReservaDirectaRequest
        {
            AnfitrionId = "anf-001",
            Huesped = "Juan Perez",
            FechaReserva = DateTime.UtcNow.AddDays(1),
            Monto = 100
        };

        var (success, error) =
            await _sut.RegistrarReservaAsync(request);

        Assert.False(success);
        Assert.Contains("fecha", error!,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RegistrarReserva_Valida_GuardaReserva()
    {
        var request = new ReservaDirectaRequest
        {
            AnfitrionId = "anf-001",
            Huesped = "Juan Perez",
            FechaReserva = DateTime.UtcNow,
            Monto = 100
        };

        var (success, error) =
            await _sut.RegistrarReservaAsync(request);

        Assert.True(success);
        Assert.Null(error);

        await _repository.Received(1)
            .AddReservaAsync(
                Arg.Any<ReservaDirecta>());
    }

    [Fact]
    public async Task RegistrarGasto_MontoInvalido_RetornaError()
    {
        var request = new GastoOperativoRequest
        {
            UsuarioId = Guid.NewGuid(),
            Proveedor = "Proveedor",
            NumeroFactura = "FAC001",
            FechaEmision = DateOnly.FromDateTime(DateTime.UtcNow),
            MontoTotal = -10
        };

        var (success, _) =
            await _sut.RegistrarGastoAsync(request);

        Assert.False(success);
    }

    [Fact]
    public async Task RegistrarGasto_Valido_GuardaGasto()
    {
        const string descripcion = "<script>alert(1)</script>";
        var request = new GastoOperativoRequest
        {
            UsuarioId = Guid.NewGuid(),
            Proveedor = "Proveedor",
            NumeroFactura = "FAC001",
            FechaEmision = DateOnly.FromDateTime(DateTime.UtcNow),
            MontoTotal = 5000,
            Descripcion = descripcion
        };

        var (success, error) =
            await _sut.RegistrarGastoAsync(request);

        Assert.True(success);
        Assert.Null(error);

        await _repository.Received(1)
            .AddGastoAsync(
                Arg.Is<FiscalHost.Api.CR.Models.Entities.Operations.Gasto>(
                    gasto => gasto.Descripcion == descripcion));
    }

    [Fact]
    public async Task SubirComprobanteGastoAsync_OcrFalla_LanzaBusinessRuleValidationExceptionConDatosExtraidos()
    {
        // Arrange
        var request = new UploadComprobanteRequest
        {
            UsuarioId = Guid.NewGuid(),
            PropiedadId = Guid.NewGuid(),
            Comprobante = Substitute.For<IFormFile>()
        };
        request.Comprobante.Length.Returns(1024);
        request.Comprobante.FileName.Returns("factura.pdf");
        request.Comprobante.ContentType.Returns("application/pdf");

        // OCR falla (Exitoso = false) pero devuelve datos parciales
        var ocrResult = new OcrResult
        {
            Exitoso = false,
            Proveedor = "Proveedor Parcial",
            NumeroFactura = null,
            FechaEmision = null,
            MontoTotal = null,
            TextoExtraidoRaw = "Texto ilegible..."
        };
        _ocrService.ExtractMetadataAsync(Arg.Any<Stream>(), Arg.Any<string>())
            .Returns(ocrResult);

        _blobStorageService.UploadAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns("uploads/comprobantes/test.pdf");

        // Act & Assert: Debe lanzar BusinessRuleValidationException con detalles del OCR
        var ex = await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
            _sut.SubirComprobanteGastoAsync(request));

        Assert.Equal("El OCR no pudo extraer los datos del comprobante. Complete los campos manualmente.", ex.Message);
        Assert.NotNull(ex.ErrorDetails);
        
        // Verificar que NO se guardó el gasto (porque OCR falló)
        await _repository.DidNotReceive().AddGastoAsync(Arg.Any<Gasto>());
    }

    [Fact]
    public async Task SubirComprobanteGastoAsync_OcrExitoso_GuardaGastoConCreditoFiscal()
    {
        // Arrange
        var request = new UploadComprobanteRequest
        {
            UsuarioId = Guid.NewGuid(),
            PropiedadId = Guid.NewGuid(),
            Comprobante = Substitute.For<IFormFile>()
        };
        request.Comprobante.Length.Returns(1024);
        request.Comprobante.FileName.Returns("factura.pdf");
        request.Comprobante.ContentType.Returns("application/pdf");

        var ocrResult = new OcrResult
        {
            Exitoso = true,
            Proveedor = "Proveedor Completo",
            NumeroFactura = "FAC-123",
            FechaEmision = DateOnly.FromDateTime(DateTime.UtcNow),
            MontoTotal = 50000m,
            TextoExtraidoRaw = "Texto completo..."
        };
        _ocrService.ExtractMetadataAsync(Arg.Any<Stream>(), Arg.Any<string>())
            .Returns(ocrResult);

        _blobStorageService.UploadAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns("uploads/comprobantes/test.pdf");

        _repository.ExisteGastoDuplicadoAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(false);

        // Act
        var (success, error) = await _sut.SubirComprobanteGastoAsync(request);

        // Assert
        Assert.True(success);
        Assert.Null(error);
        
        await _repository.Received(1).AddGastoAsync(Arg.Is<Gasto>(g =>
            g.Proveedor == "Proveedor Completo" &&
            g.NumeroFactura == "FAC-123" &&
            g.EsCreditoFiscalValido == true &&
            g.EstadoOcr == EstadoOcr.PROCESADO));
    }
}
