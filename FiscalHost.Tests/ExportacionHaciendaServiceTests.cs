using System.Text;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Enums.Operations;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using NSubstitute;

namespace FiscalHost.Tests;

public class ExportacionHaciendaServiceTests
{
    private readonly IExportacionHaciendaRepository _repository =
        Substitute.For<IExportacionHaciendaRepository>();

    private readonly ExportacionHaciendaService _sut;

    public ExportacionHaciendaServiceTests()
    {
        _sut = new ExportacionHaciendaService(_repository);
    }

    [Fact]
    public async Task Exportar_XmlConDatos_GeneraContenido()
    {
        var usuarioId = Guid.NewGuid();
        _repository.GetReservasAsync(usuarioId, 2026, 1)
            .Returns([BuildReserva(usuarioId)]);
        _repository.GetGastosAsync(usuarioId, 2026, 1)
            .Returns([]);

        var result = await _sut.ExportarAsync(BuildRequest(usuarioId, "XML"));

        Assert.True(result.Success);
        Assert.Equal("application/xml", result.TipoMime);
        var xml = Encoding.UTF8.GetString(Convert.FromBase64String(result.ContenidoBase64));
        Assert.Contains("DeclaracionHacienda", xml);
        Assert.Contains("Ingreso", xml);
    }

    [Fact]
    public async Task Exportar_CsvConDatos_GeneraArchivoParaContador()
    {
        var usuarioId = Guid.NewGuid();
        _repository.GetReservasAsync(usuarioId, 2026, 1)
            .Returns([BuildReserva(usuarioId)]);
        _repository.GetGastosAsync(usuarioId, 2026, 1)
            .Returns([BuildGasto(usuarioId)]);

        var result = await _sut.ExportarAsync(BuildRequest(usuarioId, "CSV", "MOVIMIENTOS"));

        Assert.True(result.Success);
        Assert.Equal("text/csv", result.TipoMime);
        var csv = Encoding.UTF8.GetString(Convert.FromBase64String(result.ContenidoBase64));
        Assert.Contains("INGRESO", csv);
        Assert.Contains("GASTO", csv);
    }

    [Fact]
    public async Task Exportar_SinDatos_RetornaError()
    {
        var usuarioId = Guid.NewGuid();
        _repository.GetReservasAsync(usuarioId, 2026, 1).Returns([]);
        _repository.GetGastosAsync(usuarioId, 2026, 1).Returns([]);

        var result = await _sut.ExportarAsync(BuildRequest(usuarioId, "XML"));

        Assert.False(result.Success);
        Assert.Contains("No existen datos", result.Mensaje);
    }

    [Fact]
    public async Task Exportar_Protegido_CifraContenido()
    {
        var usuarioId = Guid.NewGuid();
        _repository.GetReservasAsync(usuarioId, 2026, 1)
            .Returns([BuildReserva(usuarioId)]);
        _repository.GetGastosAsync(usuarioId, 2026, 1)
            .Returns([]);

        var result = await _sut.ExportarAsync(new ExportacionHaciendaRequest
        {
            UsuarioId = usuarioId,
            AnioFiscal = 2026,
            Mes = 1,
            Formato = "XML",
            TipoContenido = "DECLARACION",
            ProtegerConContrasena = true,
            Contrasena = "ClaveTemporal123"
        });

        Assert.True(result.Success);
        Assert.True(result.EstaProtegido);
        Assert.Equal("application/octet-stream", result.TipoMime);
    }

    private static ExportacionHaciendaRequest BuildRequest(
        Guid usuarioId,
        string formato,
        string tipoContenido = "DECLARACION") => new()
    {
        UsuarioId = usuarioId,
        AnioFiscal = 2026,
        Mes = 1,
        Formato = formato,
        TipoContenido = tipoContenido
    };

    private static Reserva BuildReserva(Guid usuarioId) => new()
    {
        ReservaId = Guid.NewGuid(),
        UsuarioId = usuarioId,
        FechaInicio = new DateTime(2026, 1, 1),
        FechaFin = new DateTime(2026, 1, 5),
        PeriodoFiscalAnio = 2026,
        PeriodoFiscalMes = 1,
        MontoBruto = 1000,
        MontoColones = 1000,
        MontoGravado = 1000,
        MontoIvaCalculado = 130,
        PlataformaOrigen = PlataformaOrigen.AIRBNB,
        ReferenciaPlataforma = "AIR-001"
    };

    private static Gasto BuildGasto(Guid usuarioId) => new()
    {
        GastoId = Guid.NewGuid(),
        UsuarioId = usuarioId,
        FechaEmision = new DateOnly(2026, 1, 3),
        Proveedor = "Proveedor Demo",
        NumeroFactura = "FAC-001",
        PeriodoFiscalAnio = 2026,
        PeriodoFiscalMes = 1,
        MontoTotal = 100,
        MontoColones = 100,
        MontoIvaSoportado = 13,
        EsCreditoFiscalValido = true
    };
}
