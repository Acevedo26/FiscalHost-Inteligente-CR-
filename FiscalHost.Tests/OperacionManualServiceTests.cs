using FiscalHost.Api.CR.Models.DTOs;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using NSubstitute;

namespace FiscalHost.Tests;

public class OperacionManualServiceTests
{
    private readonly IOperacionManualRepository _repository =
        Substitute.For<IOperacionManualRepository>();

    private readonly OperacionManualService _sut;

    public OperacionManualServiceTests()
    {
        _sut = new OperacionManualService(_repository);
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
            AnfitrionId = "anf-001",
            Proveedor = "Proveedor",
            NumeroFactura = "FAC001",
            FechaGasto = DateTime.UtcNow,
            Monto = -10
        };

        var (success, _) =
            await _sut.RegistrarGastoAsync(request);

        Assert.False(success);
    }

    [Fact]
    public async Task RegistrarGasto_Valido_GuardaGasto()
    {
        var request = new GastoOperativoRequest
        {
            AnfitrionId = "anf-001",
            Proveedor = "Proveedor",
            NumeroFactura = "FAC001",
            FechaGasto = DateTime.UtcNow,
            Monto = 5000
        };

        var (success, error) =
            await _sut.RegistrarGastoAsync(request);

        Assert.True(success);
        Assert.Null(error);

        await _repository.Received(1)
            .AddGastoAsync(
                Arg.Any<GastoOperativo>());
    }
}