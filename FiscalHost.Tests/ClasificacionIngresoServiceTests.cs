using FiscalHost.Api.CR.Models.DTOs.Operations.Requests;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Enums.Operations;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using NSubstitute;

namespace FiscalHost.Tests;

public class ClasificacionIngresoServiceTests
{
    private readonly IClasificacionIngresoRepository _repository =
        Substitute.For<IClasificacionIngresoRepository>();

    private readonly ClasificacionIngresoService _sut;

    public ClasificacionIngresoServiceTests()
    {
        _sut = new ClasificacionIngresoService(_repository);
    }

    [Fact]
    public async Task Clasificar_EstanciaMenorA30Dias_RetornaGravado13()
    {
        var request = BuildRequest(fechaEntrada: DateTime.UtcNow.AddDays(-10));

        var (success, error, data) = await _sut.ClasificarAsync(request);

        Assert.True(success);
        Assert.Null(error);
        Assert.Equal("Gravado 13% IVA", data!.ClasificacionIva);
        Assert.Equal(130, data.MontoIva);
        Assert.Equal(850, data.BaseImponibleRenta);
        Assert.Equal(127.50m, data.ImpuestoRenta);
    }

    [Fact]
    public async Task Clasificar_EstanciaMayorOIgualA30DiasYResidente_RetornaExento()
    {
        var request = BuildRequest(fechaEntrada: DateTime.UtcNow.AddDays(-35));

        var (_, _, data) = await _sut.ClasificarAsync(request);

        Assert.Equal("Exento de IVA", data!.ClasificacionIva);
        Assert.Equal(0, data.MontoIva);
    }

    [Fact]
    public async Task Clasificar_FuenteExtranjeraSinFactura_AplicaRetencion()
    {
        var request = BuildRequest(
            fuenteIngreso: FuenteIngreso.Extranjera,
            tieneFacturaElectronicaNacional: false);

        var (_, _, data) = await _sut.ClasificarAsync(request);

        Assert.Equal(150, data!.MontoRetencion);
    }

    [Fact]
    public async Task Clasificar_MontoInvalido_RetornaError()
    {
        var request = BuildRequest(montoBruto: 0);

        var (success, error, _) = await _sut.ClasificarAsync(request);

        Assert.False(success);
        Assert.Contains("monto", error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Reclasificar_SinJustificacion_RetornaError()
    {
        var request = new ReclasificacionIngresoRequest
        {
            UsuarioId = "usr-001",
            ClasificacionIva = ClasificacionIva.Exento,
            Justificacion = ""
        };

        var (success, error, _) = await _sut.ReclasificarAsync(1, request);

        Assert.False(success);
        Assert.Contains("justificacion", error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Reclasificar_ConJustificacion_RegistraAuditoria()
    {
        _repository.GetByIdAsync(1).Returns(new ClasificacionIngreso
        {
            Id = 1,
            AnfitrionId = "anf-001",
            DiasEstancia = 10,
            MontoBruto = 1000,
            FuenteIngreso = FuenteIngreso.Nacional,
            ClasificacionIva = ClasificacionIva.Gravado13
        });

        var request = new ReclasificacionIngresoRequest
        {
            UsuarioId = "usr-001",
            ClasificacionIva = ClasificacionIva.Exento,
            Justificacion = "Contrato de estancia prolongada validado."
        };

        var (success, _, data) = await _sut.ReclasificarAsync(1, request);

        Assert.True(success);
        Assert.True(data!.ReclasificadoManualmente);
        Assert.Equal("Exento de IVA", data.ClasificacionIva);
        await _repository.Received(1)
            .AddAuditoriaAsync(Arg.Any<AuditoriaClasificacionIngreso>());
    }

    private static ClasificarIngresoRequest BuildRequest(
        DateTime? fechaEntrada = null,
        decimal montoBruto = 1000,
        FuenteIngreso fuenteIngreso = FuenteIngreso.Nacional,
        bool tieneFacturaElectronicaNacional = false) => new()
    {
        AnfitrionId = "anf-001",
        FechaEntrada = fechaEntrada ?? DateTime.UtcNow.AddDays(-5),
        FechaSalida = DateTime.UtcNow,
        MontoBruto = montoBruto,
        FuenteIngreso = fuenteIngreso,
        TieneFacturaElectronicaNacional = tieneFacturaElectronicaNacional,
        HuespedResidente = true
    };
}
