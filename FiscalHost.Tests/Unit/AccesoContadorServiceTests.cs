using FiscalHost.Api.CR.Models.Entities.Identity;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Moq;
using Xunit;

namespace FiscalHost.Tests.Unit;

public class AccesoContadorServiceTests
{
    private readonly Mock<IAccesoContadorRepository> _repositoryMock = new(MockBehavior.Default);
    private readonly Mock<INotificacionService> _notificacionesMock = new(MockBehavior.Default);
    private readonly AccesoContadorService _service;

    public AccesoContadorServiceTests()
    {
        _repositoryMock.Setup(x => x.GetPorVencerAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
            .ReturnsAsync(new List<AccesoContador>());
        _repositoryMock.Setup(x => x.GetExpiradosAsync(It.IsAny<DateTimeOffset>()))
            .ReturnsAsync(new List<AccesoContador>());
        _service = new AccesoContadorService(_repositoryMock.Object, _notificacionesMock.Object);
    }

    [Fact]
    public async Task ProcesarExpiracionesAsync_DebeNotificarA7DiasYA1Dia_YCambiarEstadoExpirados()
    {
        // Arrange
        var baseTime = DateTimeOffset.UtcNow;
        var anfitrionId = Guid.NewGuid();

        // Acceso que vence en 7 días (debe notificar aviso 7 días)
        var acceso7dias = new AccesoContador
        {
            AccesoId = Guid.NewGuid(),
            AnfitrionId = anfitrionId,
            CorreoContador = "contador7@test.com",
            Estado = "ACTIVO",
            FechaExpiracion = baseTime.AddDays(7)
        };

        // Acceso que vence en 1 día (debe notificar aviso 1 día)
        var acceso1dia = new AccesoContador
        {
            AccesoId = Guid.NewGuid(),
            AnfitrionId = anfitrionId,
            CorreoContador = "contador1@test.com",
            Estado = "ACTIVO",
            FechaExpiracion = baseTime.AddDays(1)
        };

        // Acceso ya expirado (debe cambiar a EXPIRADO)
        var accesoExpirado = new AccesoContador
        {
            AccesoId = Guid.NewGuid(),
            AnfitrionId = anfitrionId,
            CorreoContador = "contador_exp@test.com",
            Estado = "ACTIVO",
            FechaExpiracion = baseTime.AddDays(-1)
        };

        // Acceso que vence en 3 días (no debe notificar aún - solo 7 y 1 día)
        var acceso3dias = new AccesoContador
        {
            AccesoId = Guid.NewGuid(),
            AnfitrionId = anfitrionId,
            CorreoContador = "contador3@test.com",
            Estado = "ACTIVO",
            FechaExpiracion = baseTime.AddDays(3)
        };

        // Configurar mock para devolver TODOS los accesos por vencer en cualquier rango
        // El servicio filtrará internamente por días restantes
        var todosPorVencer = new List<AccesoContador> { acceso7dias, acceso1dia, acceso3dias };
        
        _repositoryMock.Setup(x => x.GetPorVencerAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
            .ReturnsAsync(todosPorVencer);

        _repositoryMock.Setup(x => x.GetExpiradosAsync(It.IsAny<DateTimeOffset>()))
            .ReturnsAsync([accesoExpirado]);

        // Act
        var expiradosCount = await _service.ProcesarExpiracionesAsync();

        // Assert
        // Debe notificar a 7 días (solo acceso7dias tiene ~7 días restantes)
        _notificacionesMock.Verify(x => x.NotificarAsync(
            anfitrionId.ToString(),
            It.Is<string>(msg => msg.Contains("7 día") && msg.Contains("contador7@test.com"))),
            Times.Once);

        // Debe notificar a 1 día (solo acceso1dia tiene ~1 día restante; mensaje usa "mañana")
        _notificacionesMock.Verify(x => x.NotificarAsync(
            anfitrionId.ToString(),
            It.Is<string>(msg => msg.Contains("mañana") && msg.Contains("contador1@test.com"))),
            Times.Once);

        // NO debe notificar a 3 días (acceso3dias tiene ~3 días, no 7 ni 1)
        _notificacionesMock.Verify(x => x.NotificarAsync(
            anfitrionId.ToString(),
            It.Is<string>(msg => msg.Contains("contador3@test.com"))),
            Times.Never);

        // Debe expirar el acceso vencido
        Assert.Equal("EXPIRADO", accesoExpirado.Estado);
        Assert.Equal(1, expiradosCount);

        // Debe guardar auditoría para el expirado
        _repositoryMock.Verify(x => x.AddAuditoriaAsync(It.IsAny<FiscalHost.Api.CR.Models.Entities.Audit.AuditoriaOperacion>()), Times.Once);
        _repositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ProcesarExpiracionesAsync_SinAccesosPorVencerNiExpirados_RetornaCero()
    {
        // Arrange - los defaults en el constructor ya devuelven listas vacías
        // Act
        var expiradosCount = await _service.ProcesarExpiracionesAsync();

        // Assert
        Assert.Equal(0, expiradosCount);
        _notificacionesMock.Verify(x => x.NotificarAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}