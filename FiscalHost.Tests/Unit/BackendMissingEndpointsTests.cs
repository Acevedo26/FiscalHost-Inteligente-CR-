using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Controllers;
using FiscalHost.Api.CR.Models.DTOs.Identity.Requests;
using FiscalHost.Api.CR.Models.DTOs.Identity.Responses;
using FiscalHost.Api.CR.Models.DTOs.Operations.Requests;
using FiscalHost.Api.CR.Models.DTOs.Operations.Responses;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;
using FiscalHost.Api.CR.Models.Entities.Identity;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Enums.Identity;
using FiscalHost.Api.CR.Models.Enums.Operations;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace FiscalHost.Tests.Unit;

public class BackendMissingEndpointsTests
{
    private static ControllerContext CreateContextWithUser(Guid usuarioId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
            new Claim("usuario_id", usuarioId.ToString()),
            new Claim("sub", usuarioId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task ObligacionesTributarias_ListarMisObligaciones_RetornaOkConLista()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IObligacionTributariaService>();
        var controller = new ObligacionesTributariasController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        var expected = new List<ObligacionTributariaDto>
        {
            new ObligacionTributariaDto
            {
                ObligacionId = Guid.NewGuid(),
                Descripcion = "Declaracion IVA",
                MontoCapital = 50000,
                DiasVencido = 5
            }
        };

        serviceMock.ListarPorUsuarioAsync(usuarioId, Arg.Any<DateOnly?>())
            .Returns(expected);

        // Act
        var result = await controller.ListarMisObligaciones();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<ObligacionTributariaDto>>(okResult.Value);
        Assert.Single(list);
    }

    [Fact]
    public async Task OperacionesManuales_GetOperaciones_RetornaOkConEstructuraUnificada()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IOperacionManualService>();
        var controller = new OperacionesManualesController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        serviceMock.GetGastosAsync(usuarioId).Returns(new List<GastoDto>
        {
            new GastoDto { GastoId = Guid.NewGuid(), Proveedor = "ICE", MontoTotal = 15000, FechaEmision = new DateOnly(2026, 4, 1) }
        });

        serviceMock.GetReservasAsync(usuarioId).Returns(new List<ReservaDto>
        {
            new ReservaDto { ReservaId = Guid.NewGuid(), NombreHuesped = "Ana Gomez", MontoColones = 45000, FechaInicio = DateTime.UtcNow }
        });

        // Act
        var result = await controller.GetOperaciones();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task OperacionesManuales_CrearOperacion_Gasto_Retorna201Created()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IOperacionManualService>();
        serviceMock.RegistrarGastoAsync(Arg.Any<GastoOperativoRequest>())
            .Returns((true, null));

        var controller = new OperacionesManualesController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        var request = new OperacionUnificadaRequest
        {
            Tipo = "GASTO",
            Monto = 25000,
            Concepto = "Reparacion A/C",
            Proveedor = "Tecnico SA"
        };

        // Act
        var result = await controller.CrearOperacion(request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status201Created, statusResult.StatusCode);
    }

    [Fact]
    public async Task OperacionesManuales_CrearOperacion_Ingreso_Retorna201Created()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IOperacionManualService>();
        serviceMock.RegistrarReservaAsync(Arg.Any<ReservaDirectaRequest>())
            .Returns((true, null));

        var controller = new OperacionesManualesController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        var request = new OperacionUnificadaRequest
        {
            Tipo = "INGRESO",
            Monto = 80000,
            Concepto = "Reserva Directa",
            Huesped = "Carlos Mendoza"
        };

        // Act
        var result = await controller.CrearOperacion(request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status201Created, statusResult.StatusCode);
    }

    [Fact]
    public async Task OperacionesManuales_ActualizarReserva_ConRazonCambio_Retorna200Ok()
    {
        // Arrange
        var serviceMock = Substitute.For<IOperacionManualService>();
        var reservaId = Guid.NewGuid().ToString();
        var request = new UpdateReservaRequest
        {
            Monto = 120000,
            RazonCambio = "Ajuste de noches adicionales por el huésped"
        };

        serviceMock.ActualizarReservaAsync(reservaId, request)
            .Returns((true, null));

        var controller = new OperacionesManualesController(serviceMock);

        // Act
        var result = await controller.ActualizarReserva(reservaId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task OperacionesManuales_ActualizarReserva_SinRazon_Retorna422UnprocessableEntity()
    {
        // Arrange
        var serviceMock = Substitute.For<IOperacionManualService>();
        var controller = new OperacionesManualesController(serviceMock);
        var request = new UpdateReservaRequest
        {
            Monto = 120000
            // Sin RazonCambio
        };

        // Act
        var result = await controller.ActualizarReserva(Guid.NewGuid().ToString(), request);

        // Assert
        Assert.IsType<UnprocessableEntityObjectResult>(result);
    }

    [Fact]
    public async Task OperacionesManuales_EliminarReserva_ConRazonCambio_Retorna200Ok()
    {
        // Arrange
        var serviceMock = Substitute.For<IOperacionManualService>();
        var reservaId = Guid.NewGuid().ToString();
        var request = new DeleteReservaRequest
        {
            RazonCambio = "Cancelacion solicitada formalmente por el cliente"
        };

        serviceMock.EliminarReservaAsync(reservaId, request)
            .Returns((true, null));

        var controller = new OperacionesManualesController(serviceMock);

        // Act
        var result = await controller.EliminarReserva(reservaId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task OperacionesManuales_EliminarReserva_SinRazon_Retorna422UnprocessableEntity()
    {
        // Arrange
        var serviceMock = Substitute.For<IOperacionManualService>();
        var controller = new OperacionesManualesController(serviceMock);
        var request = new DeleteReservaRequest();

        // Act
        var result = await controller.EliminarReserva(Guid.NewGuid().ToString(), request);

        // Assert
        Assert.IsType<UnprocessableEntityObjectResult>(result);
    }

    [Fact]
    public async Task AccesosContadores_GetMisAccesos_RetornaOkConAccesos()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IAccesoContadorService>();
        serviceMock.GetByAnfitrionAsync(usuarioId)
            .Returns(new List<AccesoContadorResponse>
            {
                new AccesoContadorResponse
                {
                    AccesoId = Guid.NewGuid(),
                    CorreoContador = "contador@test.com",
                    Estado = "ACTIVO"
                }
            });

        var controller = new AccesosContadoresController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        // Act
        var result = await controller.GetMisAccesos();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<AccesoContadorResponse>>(okResult.Value);
        Assert.Single(list);
    }

    [Fact]
    public async Task Usuarios_GetMe_RetornaOkConUsuario()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IUsuarioService>();
        serviceMock.ObtenerPorIdAsync(usuarioId)
            .Returns(new UsuarioDto
            {
                UsuarioId = usuarioId,
                NombreCompleto = "Juan Perez",
                CorreoElectronico = "juan@perez.cr"
            });

        var controller = new UsuarioController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        // Act
        var result = await controller.GetMe();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var user = Assert.IsType<UsuarioDto>(okResult.Value);
        Assert.Equal(usuarioId, user.UsuarioId);
    }

    [Fact]
    public async Task Usuarios_PreferenciasNotificacion_Me_ObtenerYActualizar()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IUsuarioService>();
        var prefs = new PreferenciasNotificacionDto
        {
            CanalAlertas = Api.CR.Models.Enums.Communication.CanalNotificacion.CORREO
        };

        serviceMock.ObtenerPreferenciasNotificacionAsync(usuarioId)
            .Returns(prefs);

        var updateReq = new ActualizarPreferenciasNotificacionRequest
        {
            CanalAlertas = Api.CR.Models.Enums.Communication.CanalNotificacion.AMBOS
        };

        serviceMock.ActualizarPreferenciasNotificacionAsync(usuarioId, updateReq)
            .Returns((true, null, prefs));

        var controller = new UsuarioController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        // Act - GET
        var getResult = await controller.ObtenerMisPreferenciasNotificacion();
        var okGet = Assert.IsType<OkObjectResult>(getResult);
        Assert.Equal(prefs, okGet.Value);

        // Act - PUT
        var putResult = await controller.ActualizarMisPreferenciasNotificacion(updateReq);
        var okPut = Assert.IsType<OkObjectResult>(putResult);
        Assert.NotNull(okPut.Value);
    }

    [Fact]
    public void ModelAliases_LoginRequest_Y_RegistroRequest_ResuelvenCorrectamente()
    {
        // Test LoginRequest
        var login = new LoginRequest
        {
            Correo = "usuario@correo.cr",
            Clave = "MiClaveSegura123!"
        };
        Assert.Equal("MiClaveSegura123!", login.Contrasena);

        // Test RegistroUsuarioRequest
        var registro = new RegistroUsuarioRequest
        {
            Nombre = "Maria Solano",
            Correo = "maria@solano.cr",
            Clave = "ClaveSegura#456",
            Identificacion = "111220334"
        };
        Assert.Equal("Maria Solano", registro.NombreCompleto);
        Assert.Equal("maria@solano.cr", registro.CorreoElectronico);
        Assert.Equal("ClaveSegura#456", registro.Contrasena);
        Assert.Equal("111220334", registro.NumeroIdentificacion);
        Assert.Equal("FISICA", registro.TipoIdentificacion);
    }
}
