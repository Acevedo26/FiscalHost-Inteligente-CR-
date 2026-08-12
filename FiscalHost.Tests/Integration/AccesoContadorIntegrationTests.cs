using FiscalHost.Api.CR.Data;
using FiscalHost.Api.CR.Models.DTOs.Identity.Requests;
using FiscalHost.Api.CR.Models.Enums.Identity;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace FiscalHost.Tests.Integration;

public class AccesoContadorIntegrationTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly AccesoContadorService _sut;

    public AccesoContadorIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _sut = new AccesoContadorService(
            new AccesoContadorRepository(_db),
            Substitute.For<INotificacionService>());
    }

    [Fact]
    public async Task InvitarYRevocar_GuardaAccesoYAuditoria()
    {
        var anfitrionId = Guid.NewGuid();
        _db.Usuarios.Add(new Usuario
        {
            UsuarioId = anfitrionId,
            CorreoElectronico = "anfitrion@demo.com",
            RolPrincipal = RolUsuario.ANFITRION
        });
        await _db.SaveChangesAsync();

        var invitacion = await _sut.InvitarAsync(new InvitarContadorRequest
        {
            AnfitrionId = anfitrionId,
            CorreoContador = "contador@demo.com",
            PuedeVerIngresos = true
        });

        await _sut.RevocarAsync(invitacion.data!.AccesoId, new RevocarAccesoContadorRequest
        {
            AnfitrionId = anfitrionId,
            Justificacion = "Cambio de contador autorizado por el anfitrion."
        });

        Assert.Single(_db.AccesosContadores);
        Assert.Equal("REVOCADO", _db.AccesosContadores.Single().Estado);
        Assert.Equal(2, _db.AuditoriasOperacion.Count());
    }

    public void Dispose() => _db.Dispose();
}
