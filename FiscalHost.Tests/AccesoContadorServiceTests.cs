using FiscalHost.Api.CR.Models.DTOs.Identity.Requests;
using FiscalHost.Api.CR.Models.Entities.Identity;
using FiscalHost.Api.CR.Models.Enums.Identity;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using NSubstitute;

namespace FiscalHost.Tests;

public class AccesoContadorServiceTests
{
    private readonly IAccesoContadorRepository _repository =
        Substitute.For<IAccesoContadorRepository>();
    private readonly INotificacionService _notificaciones =
        Substitute.For<INotificacionService>();
    private readonly AccesoContadorService _sut;

    public AccesoContadorServiceTests()
    {
        _sut = new AccesoContadorService(_repository, _notificaciones);
    }

    [Fact]
    public async Task Invitar_CorreoInvalido_RetornaError()
    {
        var result = await _sut.InvitarAsync(new InvitarContadorRequest
        {
            AnfitrionId = Guid.NewGuid(),
            CorreoContador = "correo-invalido"
        });

        Assert.False(result.success);
        Assert.Contains("correo", result.error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Invitar_Valido_CreaAccesoYAuditoria()
    {
        var anfitrionId = Guid.NewGuid();
        _repository.GetUsuarioAsync(anfitrionId).Returns(new Usuario
        {
            UsuarioId = anfitrionId,
            CorreoElectronico = "anfitrion@demo.com",
            RolPrincipal = RolUsuario.ANFITRION
        });

        var result = await _sut.InvitarAsync(new InvitarContadorRequest
        {
            AnfitrionId = anfitrionId,
            CorreoContador = "contador@demo.com",
            PuedeVerIngresos = true,
            PuedeVerGastos = false,
            PuedeGenerarReportes = true
        });

        Assert.True(result.success);
        Assert.NotNull(result.data);
        Assert.True(result.data!.PuedeVerIngresos);
        Assert.False(result.data.PuedeVerGastos);
        await _repository.Received(1).AddAsync(Arg.Any<AccesoContador>());
        await _repository.Received(1).AddAuditoriaAsync(Arg.Any<AuditoriaOperacion>());
    }

    [Fact]
    public async Task Revocar_SinJustificacion_RetornaError()
    {
        var result = await _sut.RevocarAsync(Guid.NewGuid(), new RevocarAccesoContadorRequest
        {
            AnfitrionId = Guid.NewGuid(),
            Justificacion = ""
        });

        Assert.False(result.success);
        Assert.Contains("justificacion", result.error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidarPermiso_SinAccesoActivo_Deniega()
    {
        var (autorizado, mensaje) = await _sut.ValidarPermisoAsync(
            Guid.NewGuid(),
            "contador@demo.com",
            "INGRESOS");

        Assert.False(autorizado);
        Assert.Contains("insuficientes", mensaje, StringComparison.OrdinalIgnoreCase);
    }
}
