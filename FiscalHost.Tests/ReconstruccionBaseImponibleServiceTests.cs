using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Entities.TaxIntelligence;
using FiscalHost.Api.CR.Models.Enums.Operations;
using FiscalHost.Api.CR.Models.Enums.TaxIntelligence;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using NSubstitute;

namespace FiscalHost.Tests;

public class ReconstruccionBaseImponibleServiceTests
{
    private readonly IReconstruccionBaseImponibleRepository _repository =
        Substitute.For<IReconstruccionBaseImponibleRepository>();

    private readonly ReconstruccionBaseImponibleService _sut;

    public ReconstruccionBaseImponibleServiceTests()
    {
        _sut = new ReconstruccionBaseImponibleService(_repository);
    }

    [Fact]
    public async Task Reconstruir_ConDatosHistoricos_ReconstruyeBaseMensual()
    {
        var usuarioId = Guid.NewGuid();
        _repository.GetReservasPorAnioAsync(usuarioId, 2025)
            .Returns([BuildReserva(usuarioId, 1, 1000, ClasificacionFiscal.GRAVADO)]);
        _repository.GetPeriodosPorAnioAsync(2025)
            .Returns([BuildPeriodo(1)]);

        var result = await _sut.ReconstruirAsync(new ReconstruccionBaseImponibleRequest
        {
            UsuarioId = usuarioId,
            AnioFiscal = 2025,
            ContinuarConDatosIncompletos = true
        });

        Assert.True(result.Success);
        Assert.Equal(1000, result.Consolidado.TotalIngresosBrutos);
        Assert.Equal(130, result.Consolidado.TotalDebitoFiscal);
        Assert.Contains(result.BasesMensuales, m => m.Mes == 1 && m.TieneDatos);
    }

    [Fact]
    public async Task Reconstruir_DatosIncompletosSinContinuar_RetornaAdvertencia()
    {
        var usuarioId = Guid.NewGuid();
        _repository.GetReservasPorAnioAsync(usuarioId, 2025)
            .Returns([BuildReserva(usuarioId, 1, 1000, ClasificacionFiscal.GRAVADO)]);
        _repository.GetPeriodosPorAnioAsync(2025)
            .Returns([BuildPeriodo(1)]);

        var result = await _sut.ReconstruirAsync(new ReconstruccionBaseImponibleRequest
        {
            UsuarioId = usuarioId,
            AnioFiscal = 2025,
            ContinuarConDatosIncompletos = false
        });

        Assert.False(result.Success);
        Assert.Contains(2, result.MesesSinDatos);
        await _repository.DidNotReceive().AddCalculosAsync(Arg.Any<List<CalculoFiscal>>());
    }

    [Fact]
    public async Task Reconstruir_SinNormativaHistorica_InformaMesAfectado()
    {
        var usuarioId = Guid.NewGuid();
        _repository.GetReservasPorAnioAsync(usuarioId, 2025)
            .Returns([BuildReserva(usuarioId, 3, 500, ClasificacionFiscal.EXENTO)]);
        _repository.GetPeriodosPorAnioAsync(2025)
            .Returns([]);

        var result = await _sut.ReconstruirAsync(new ReconstruccionBaseImponibleRequest
        {
            UsuarioId = usuarioId,
            AnioFiscal = 2025,
            ContinuarConDatosIncompletos = false
        });

        Assert.False(result.Success);
        Assert.Contains(3, result.MesesSinNormativa);
    }

    private static Reserva BuildReserva(
        Guid usuarioId,
        short mes,
        decimal monto,
        ClasificacionFiscal clasificacion) => new()
    {
        ReservaId = Guid.NewGuid(),
        UsuarioId = usuarioId,
        PeriodoFiscalAnio = 2025,
        PeriodoFiscalMes = mes,
        MontoBruto = monto,
        MontoColones = monto,
        MontoGravado = clasificacion == ClasificacionFiscal.EXENTO ? 0 : monto,
        MontoExento = clasificacion == ClasificacionFiscal.EXENTO ? monto : 0,
        ClasificacionFiscal = clasificacion
    };

    private static PeriodoFiscal BuildPeriodo(short mes) => new()
    {
        PeriodoId = Guid.NewGuid(),
        Anio = 2025,
        Mes = mes,
        TipoFormulario = TipoFormulario.D104,
        TarifaIva = 0.13m,
        TarifaRentaCapital = 0.15m,
        DeduccionPlanaCapital = 0.85m
    };
}
