using FiscalHost.Api.CR.Models.DTOs.Audit.Requests;
using FiscalHost.Api.CR.Models.Entities.Audit;
using FiscalHost.Api.CR.Models.Enums.Audit;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using NSubstitute;

namespace FiscalHost.Tests;

public class AuditoriaInalterableServiceTests
{
    private readonly IAuditoriaInalterableRepository _repository =
        Substitute.For<IAuditoriaInalterableRepository>();

    private readonly AuditoriaInalterableService _sut;

    public AuditoriaInalterableServiceTests()
    {
        _sut = new AuditoriaInalterableService(_repository);
    }

    [Fact]
    public async Task Registrar_CampoSensibleSinJustificacion_RetornaError()
    {
        var result = await _sut.RegistrarAsync(new RegistrarAuditoriaRequest
        {
            Operacion = OperacionAuditoria.UPDATE,
            TablaAfectada = "usuario",
            EsCampoSensible = true
        });

        Assert.False(result.success);
        Assert.Contains("justificacion", result.error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Registrar_CambioValido_GuardaAuditoria()
    {
        var result = await _sut.RegistrarAsync(new RegistrarAuditoriaRequest
        {
            UsuarioId = Guid.NewGuid(),
            Operacion = OperacionAuditoria.UPDATE,
            TablaAfectada = "perfil_tributario",
            RegistroId = Guid.NewGuid(),
            OldValues = "{\"estado\":\"ANTERIOR\"}",
            NewValues = "{\"estado\":\"NUEVO\"}",
            CamposModificados = ["estado"],
            Justificacion = "Actualizacion solicitada por el anfitrion."
        });

        Assert.True(result.success);
        await _repository.Received(1).AddAsync(Arg.Any<AuditoriaOperacion>());
        await _repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Exportar_SinHistorial_RetornaError()
    {
        _repository.GetHistorialAsync(null, null, null).Returns([]);

        var result = await _sut.ExportarHistorialAsync(null, null, null);

        Assert.False(result.Success);
        Assert.Contains("No existen", result.Mensaje);
    }
}
