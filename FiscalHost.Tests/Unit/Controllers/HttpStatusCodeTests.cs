using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Controllers;
using FiscalHost.Api.CR.Models.DTOs.Operations.Requests;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using SimulacionParametrosDto = FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.SimulacionParametrosDto;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;
using FiscalHost.Api.CR.Models.Enums.Operations;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace FiscalHost.Tests.Unit.Controllers;

public class HttpStatusCodeTests
{
    private static ControllerContext CreateContextWithUser(Guid usuarioId, string? role = null)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
            new Claim("usuario_id", usuarioId.ToString()),
            new Claim("sub", usuarioId.ToString())
        };
        if (!string.IsNullOrEmpty(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var identity = new ClaimsIdentity(claims, "TestAuth", ClaimTypes.NameIdentifier, ClaimTypes.Role);
        var user = new ClaimsPrincipal(identity);

        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    private static ControllerContext CreateContextWithoutAuth()
    {
        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };
    }

    // ========================================================================
    // SimuladorFiscalController - POST CrearSimulacion, GET ObtenerSimulaciones
    // ========================================================================
    // NOTA: SimuladorFiscalController llama a User.GetUsuarioId() en sus acciones,
    // por lo que SIN middleware, lanza UnauthorizedAccessException si no hay usuario.
    // CON middleware (integración real), [Authorize] devuelve 401/403.

    [Fact]
    public async Task SimuladorFiscal_CrearSimulacion_SinAutenticacion_LanzaUnauthorizedAccessException()
    {
        // Arrange: Sin autenticación, User.GetUsuarioId() lanza UnauthorizedAccessException
        var serviceMock = Substitute.For<ISimulacionFiscalService>();
        var controller = new SimuladorFiscalController(serviceMock)
        {
            ControllerContext = CreateContextWithoutAuth()
        };

        var request = new CreateSimulacionFiscalRequest
        {
            Nombre = "Test Simulación",
            PeriodoBaseAnio = 2026,
            Parametros = new SimulacionParametrosDto()
        };

        // Act & Assert: El controller llama a User.GetUsuarioId() que lanza la excepción
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => controller.CrearSimulacion(request));
    }

    [Fact]
    public async Task SimuladorFiscal_CrearSimulacion_ConUsuarioAutenticado_Retorna201Created()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<ISimulacionFiscalService>();
        var controller = new SimuladorFiscalController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        var request = new CreateSimulacionFiscalRequest
        {
            Nombre = "Test Simulación",
            PeriodoBaseAnio = 2026,
            Parametros = new SimulacionParametrosDto()
        };

        serviceMock.CrearSimulacionAsync(usuarioId, request)
            .Returns(Task.FromResult(new SimulacionFiscalResponseDto
            {
                SimulacionId = Guid.NewGuid(),
                Nombre = "Test Simulación",
                PeriodoBaseAnio = 2026
            }));

        // Act
        var result = await controller.CrearSimulacion(request);

        // Assert
        var statusCode = GetStatusCode(result.Result);
        Assert.Equal(201, statusCode);
    }

    [Fact]
    public async Task SimuladorFiscal_CrearSimulacion_InvalidModel_NoValidaModelState_ControllerNoVerifica()
    {
        // Arrange: El controlador NO verifica ModelState.IsValid antes de llamar al servicio
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<ISimulacionFiscalService>();
        var controller = new SimuladorFiscalController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        controller.ModelState.AddModelError("Nombre", "Required");

        var request = new CreateSimulacionFiscalRequest
        {
            Nombre = "", // Inválido
            PeriodoBaseAnio = 2026,
            Parametros = new SimulacionParametrosDto()
        };

        // El servicio debe ser llamado aunque el modelo sea inválido (el controller no valida ModelState)
        serviceMock.CrearSimulacionAsync(usuarioId, request)
            .Returns(Task.FromResult(new SimulacionFiscalResponseDto
            {
                SimulacionId = Guid.NewGuid(),
                Nombre = "Test",
                PeriodoBaseAnio = 2026
            }));

        // Act
        var result = await controller.CrearSimulacion(request);

        // Assert: El controlador devuelve 201 porque no valida ModelState
        var statusCode = GetStatusCode(result.Result);
        Assert.Equal(201, statusCode);
    }

    [Fact]
    public async Task SimuladorFiscal_CrearSimulacion_ServiceThrowsArgumentException_Retorna400BadRequest()
    {
        // Arrange: El controller captura ArgumentException y devuelve 400
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<ISimulacionFiscalService>();
        var controller = new SimuladorFiscalController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        var request = new CreateSimulacionFiscalRequest
        {
            Nombre = "Test",
            PeriodoBaseAnio = 2026,
            Parametros = new SimulacionParametrosDto()
        };

        serviceMock.CrearSimulacionAsync(usuarioId, request)
            .Returns(Task.FromException<SimulacionFiscalResponseDto>(new ArgumentException("Parámetros inválidos")));

        // Act
        var result = await controller.CrearSimulacion(request);

        // Assert
        var statusCode = GetStatusCode(result.Result);
        Assert.Equal(400, statusCode);
    }

    [Fact]
    public async Task SimuladorFiscal_CrearSimulacion_ServiceThrowsException_SinMiddleware_LanzaExcepcion()
    {
        // Arrange: Sin GlobalExceptionMiddleware, las excepciones no manejadas burbujean
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<ISimulacionFiscalService>();
        var controller = new SimuladorFiscalController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        var request = new CreateSimulacionFiscalRequest
        {
            Nombre = "Test",
            PeriodoBaseAnio = 2026,
            Parametros = new SimulacionParametrosDto()
        };

        serviceMock.CrearSimulacionAsync(usuarioId, request)
            .Returns(Task.FromException<SimulacionFiscalResponseDto>(new InvalidOperationException("Error interno")));

        // Act & Assert: En unit tests sin middleware, la excepción no es capturada
        await Assert.ThrowsAsync<InvalidOperationException>(() => controller.CrearSimulacion(request));
    }

    [Fact]
    public async Task SimuladorFiscal_ListarSimulaciones_SinAutenticacion_LanzaUnauthorizedAccessException()
    {
        // Arrange
        var serviceMock = Substitute.For<ISimulacionFiscalService>();
        var controller = new SimuladorFiscalController(serviceMock)
        {
            ControllerContext = CreateContextWithoutAuth()
        };

        serviceMock.ListarSimulacionesAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult<IEnumerable<SimulacionFiscalResponseDto>>(new List<SimulacionFiscalResponseDto>()));

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => controller.ListarSimulaciones());
    }

    [Fact]
    public async Task SimuladorFiscal_ListarSimulaciones_ConUsuarioAutenticado_Retorna200Ok()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<ISimulacionFiscalService>();
        var controller = new SimuladorFiscalController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        serviceMock.ListarSimulacionesAsync(usuarioId)
            .Returns(Task.FromResult<IEnumerable<SimulacionFiscalResponseDto>>(new List<SimulacionFiscalResponseDto>()));

        // Act
        var result = await controller.ListarSimulaciones();

        // Assert
        var statusCode = GetStatusCode(result.Result);
        Assert.Equal(200, statusCode);
    }

    // ========================================================================
    // ObligacionesTributariasController - GET ConsultarDeuda
    // ========================================================================
    // NOTA: ObligacionesTributariasController NO llama a User.GetUsuarioId() en ConsultarDeuda.
    // El endpoint es alcanzable SIN autenticación en unit tests (sin middleware).
    // CON middleware (integración real), [Authorize] devuelve 401/403.
    // La autorización por ownership (verificar que la obligación pertenece al usuario) NO existe.

    [Fact]
    public async Task ObligacionesTributarias_ConsultarDeuda_SinAutenticacion_AlcanzableSinMiddleware_Retorna200Ok()
    {
        // Arrange: Sin middleware, [Authorize] no hace nada. El controller no llama a User.GetUsuarioId().
        var serviceMock = Substitute.For<IObligacionTributariaService>();
        var controller = new ObligacionesTributariasController(serviceMock)
        {
            ControllerContext = CreateContextWithoutAuth()
        };

        var obligacionId = Guid.NewGuid();
        serviceMock.ConsultarDeudaAsync(obligacionId)
            .Returns(Task.FromResult<ObligacionTributariaDto?>(new ObligacionTributariaDto
            {
                ObligacionId = obligacionId,
                Descripcion = "IVA",
                MontoCapital = 10000,
                FechaVencimiento = DateTime.UtcNow.AddDays(10)
            }));

        // Act
        var result = await controller.ConsultarDeuda(obligacionId);

        // Assert: En unit tests SIN middleware, el endpoint es alcanzable y devuelve 200
        // CON middleware real, [Authorize] devolvería 401
        var statusCode = GetStatusCode(result);
        Assert.Equal(200, statusCode);
    }

    [Fact]
    public async Task ObligacionesTributarias_ConsultarDeuda_ConUsuarioAutenticado_Retorna200Ok()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IObligacionTributariaService>();
        var controller = new ObligacionesTributariasController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        var obligacionId = Guid.NewGuid();
        serviceMock.ConsultarDeudaAsync(obligacionId)
            .Returns(Task.FromResult<ObligacionTributariaDto?>(new ObligacionTributariaDto
            {
                ObligacionId = obligacionId,
                Descripcion = "IVA",
                MontoCapital = 10000,
                FechaVencimiento = DateTime.UtcNow.AddDays(10)
            }));

        // Act
        var result = await controller.ConsultarDeuda(obligacionId);

        // Assert
        var statusCode = GetStatusCode(result);
        Assert.Equal(200, statusCode);
    }

    [Fact]
    public async Task ObligacionesTributarias_ConsultarDeuda_NotFound_Retorna404NotFound()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IObligacionTributariaService>();
        var controller = new ObligacionesTributariasController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        serviceMock.ConsultarDeudaAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult<ObligacionTributariaDto?>(null));

        // Act
        var result = await controller.ConsultarDeuda(Guid.NewGuid());

        // Assert
        var statusCode = GetStatusCode(result);
        Assert.Equal(404, statusCode);
    }

    [Fact]
    public async Task ObligacionesTributarias_ConsultarDeuda_InvalidVencimiento_Retorna400BadRequest()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IObligacionTributariaService>();
        var controller = new ObligacionesTributariasController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        var obligacionId = Guid.NewGuid();
        serviceMock.ConsultarDeudaAsync(obligacionId)
            .Returns(Task.FromResult<ObligacionTributariaDto?>(new ObligacionTributariaDto
            {
                ObligacionId = obligacionId,
                Descripcion = "IVA",
                MontoCapital = 10000,
                FechaVencimiento = default(DateTime) // Inválido
            }));

        // Act
        var result = await controller.ConsultarDeuda(obligacionId);

        // Assert
        var statusCode = GetStatusCode(result);
        Assert.Equal(400, statusCode);
    }

    [Fact]
    public async Task ObligacionesTributarias_ConsultarDeuda_ServiceThrows_SinMiddleware_LanzaExcepcion()
    {
        // Arrange: Sin GlobalExceptionMiddleware, la excepción burbujea
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IObligacionTributariaService>();
        var controller = new ObligacionesTributariasController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        serviceMock.ConsultarDeudaAsync(Arg.Any<Guid>())
            .Returns(Task.FromException<ObligacionTributariaDto?>(new InvalidOperationException("Error BD")));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => controller.ConsultarDeuda(Guid.NewGuid()));
    }

    // ========================================================================
    // OperacionesManualesController - PUT ActualizarReserva, DELETE EliminarReserva
    // ========================================================================
    // NOTA: OperacionesManualesController NO llama a User.GetUsuarioId() en ActualizarReserva/EliminarReserva.
    // El endpoint es alcanzable SIN autenticación en unit tests (sin middleware).
    // CON middleware (integración real), [Authorize] devuelve 401/403.
    // El userId se pasa implícitamente al service (no se usa en el controller actual).

    [Fact]
    public async Task OperacionesManuales_ActualizarReserva_SinAutenticacion_AlcanzableSinMiddleware_Retorna200Ok()
    {
        // Arrange: Sin middleware, [Authorize] no hace nada. El controller no llama a User.GetUsuarioId().
        var serviceMock = Substitute.For<IOperacionManualService>();
        var controller = new OperacionesManualesController(serviceMock)
        {
            ControllerContext = CreateContextWithoutAuth()
        };

        var request = new UpdateReservaRequest
        {
            Monto = 50000,
            RazonCambio = "Justificación válida"
        };

        serviceMock.ActualizarReservaAsync(Arg.Any<string>(), Arg.Any<UpdateReservaRequest>())
            .Returns(Task.FromResult((true, (string?)null)));

        // Act
        var result = await controller.ActualizarReserva(Guid.NewGuid().ToString(), request);

        // Assert: En unit tests SIN middleware, el endpoint es alcanzable y devuelve 200
        // CON middleware real, [Authorize] devolvería 401
        var statusCode = GetStatusCode(result);
        Assert.Equal(200, statusCode);
    }

    [Fact]
    public async Task OperacionesManuales_ActualizarReserva_ConUsuarioAutenticado_Retorna200Ok()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IOperacionManualService>();
        var controller = new OperacionesManualesController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        var request = new UpdateReservaRequest
        {
            Monto = 50000,
            RazonCambio = "Justificación válida"
        };

        serviceMock.ActualizarReservaAsync(Arg.Any<string>(), Arg.Any<UpdateReservaRequest>())
            .Returns(Task.FromResult((true, (string?)null)));

        // Act
        var result = await controller.ActualizarReserva(Guid.NewGuid().ToString(), request);

        // Assert
        var statusCode = GetStatusCode(result);
        Assert.Equal(200, statusCode);
    }

    [Fact]
    public async Task OperacionesManuales_ActualizarReserva_InvalidModel_Retorna400BadRequest()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IOperacionManualService>();
        var controller = new OperacionesManualesController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        controller.ModelState.AddModelError("Monto", "Required");

        var request = new UpdateReservaRequest
        {
            Monto = 0, // Inválido
            RazonCambio = "Justificación"
        };

        // Act
        var result = await controller.ActualizarReserva(Guid.NewGuid().ToString(), request);

        // Assert: El controller SÍ valida ModelState en ActualizarReserva
        var statusCode = GetStatusCode(result);
        Assert.Equal(400, statusCode);
    }

    [Fact]
    public async Task OperacionesManuales_ActualizarReserva_MissingRazonCambio_Retorna422UnprocessableEntity()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IOperacionManualService>();
        var controller = new OperacionesManualesController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        var request = new UpdateReservaRequest
        {
            Monto = 50000
            // Sin RazonCambio
        };

        // Act
        var result = await controller.ActualizarReserva(Guid.NewGuid().ToString(), request);

        // Assert
        var statusCode = GetStatusCode(result);
        Assert.Equal(422, statusCode);
    }

    [Fact]
    public async Task OperacionesManuales_ActualizarReserva_ServiceReturnsNotFound_Retorna422UnprocessableEntity()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IOperacionManualService>();
        var controller = new OperacionesManualesController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        var request = new UpdateReservaRequest
        {
            Monto = 50000,
            RazonCambio = "Justificación válida"
        };

        serviceMock.ActualizarReservaAsync(Arg.Any<string>(), Arg.Any<UpdateReservaRequest>())
            .Returns(Task.FromResult((false, "Reserva no encontrada.")));

        // Act
        var result = await controller.ActualizarReserva(Guid.NewGuid().ToString(), request);

        // Assert - El controller devuelve 422 para errores de negocio
        var statusCode = GetStatusCode(result);
        Assert.Equal(422, statusCode);
    }

    [Fact]
    public async Task OperacionesManuales_ActualizarReserva_ServiceThrows_SinMiddleware_LanzaExcepcion()
    {
        // Arrange: Sin GlobalExceptionMiddleware, la excepción burbujea
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IOperacionManualService>();
        var controller = new OperacionesManualesController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        var request = new UpdateReservaRequest
        {
            Monto = 50000,
            RazonCambio = "Justificación válida"
        };

        serviceMock.ActualizarReservaAsync(Arg.Any<string>(), Arg.Any<UpdateReservaRequest>())
            .Returns(Task.FromException<(bool success, string? error)>(new InvalidOperationException("Error BD")));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => controller.ActualizarReserva(Guid.NewGuid().ToString(), request));
    }

    [Fact]
    public async Task OperacionesManuales_EliminarReserva_SinAutenticacion_AlcanzableSinMiddleware_Retorna200Ok()
    {
        // Arrange: Sin middleware, [Authorize] no hace nada. El controller no llama a User.GetUsuarioId().
        var serviceMock = Substitute.For<IOperacionManualService>();
        var controller = new OperacionesManualesController(serviceMock)
        {
            ControllerContext = CreateContextWithoutAuth()
        };

        var request = new DeleteReservaRequest
        {
            RazonCambio = "Cancelación justificada"
        };

        serviceMock.EliminarReservaAsync(Arg.Any<string>(), Arg.Any<DeleteReservaRequest>())
            .Returns(Task.FromResult((true, (string?)null)));

        // Act
        var result = await controller.EliminarReserva(Guid.NewGuid().ToString(), request);

        // Assert: En unit tests SIN middleware, el endpoint es alcanzable y devuelve 200
        // CON middleware real, [Authorize] devolvería 401
        var statusCode = GetStatusCode(result);
        Assert.Equal(200, statusCode);
    }

    [Fact]
    public async Task OperacionesManuales_EliminarReserva_ConUsuarioAutenticado_Retorna200Ok()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IOperacionManualService>();
        var controller = new OperacionesManualesController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        var request = new DeleteReservaRequest
        {
            RazonCambio = "Cancelación justificada"
        };

        serviceMock.EliminarReservaAsync(Arg.Any<string>(), Arg.Any<DeleteReservaRequest>())
            .Returns(Task.FromResult((true, (string?)null)));

        // Act
        var result = await controller.EliminarReserva(Guid.NewGuid().ToString(), request);

        // Assert
        var statusCode = GetStatusCode(result);
        Assert.Equal(200, statusCode);
    }

    [Fact]
    public async Task OperacionesManuales_EliminarReserva_EmptyRazonCambio_Retorna422UnprocessableEntity()
    {
        // Arrange: El controlador valida RazonCambio ANTES que ModelState
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IOperacionManualService>();
        var controller = new OperacionesManualesController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        var request = new DeleteReservaRequest
        {
            RazonCambio = "" // Vacío -> 422 por validación de negocio
        };

        // Act
        var result = await controller.EliminarReserva(Guid.NewGuid().ToString(), request);

        // Assert: Retorna 422 porque RazonCambio está vacío (validación de Ley 8968)
        var statusCode = GetStatusCode(result);
        Assert.Equal(422, statusCode);
    }

    [Fact]
    public async Task OperacionesManuales_EliminarReserva_MissingRazonCambio_Retorna422UnprocessableEntity()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IOperacionManualService>();
        var controller = new OperacionesManualesController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        var request = new DeleteReservaRequest(); // Sin RazonCambio

        // Act
        var result = await controller.EliminarReserva(Guid.NewGuid().ToString(), request);

        // Assert
        var statusCode = GetStatusCode(result);
        Assert.Equal(422, statusCode);
    }

    [Fact]
    public async Task OperacionesManuales_EliminarReserva_ServiceReturnsNotFound_Retorna422UnprocessableEntity()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IOperacionManualService>();
        var controller = new OperacionesManualesController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        var request = new DeleteReservaRequest
        {
            RazonCambio = "Cancelación justificada"
        };

        serviceMock.EliminarReservaAsync(Arg.Any<string>(), Arg.Any<DeleteReservaRequest>())
            .Returns(Task.FromResult((false, "Reserva no encontrada.")));

        // Act
        var result = await controller.EliminarReserva(Guid.NewGuid().ToString(), request);

        // Assert
        var statusCode = GetStatusCode(result);
        Assert.Equal(422, statusCode);
    }

    [Fact]
    public async Task OperacionesManuales_EliminarReserva_ServiceThrows_SinMiddleware_LanzaExcepcion()
    {
        // Arrange: Sin GlobalExceptionMiddleware, la excepción burbujea
        var usuarioId = Guid.NewGuid();
        var serviceMock = Substitute.For<IOperacionManualService>();
        var controller = new OperacionesManualesController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        var request = new DeleteReservaRequest
        {
            RazonCambio = "Cancelación justificada"
        };

        serviceMock.EliminarReservaAsync(Arg.Any<string>(), Arg.Any<DeleteReservaRequest>())
            .Returns(Task.FromException<(bool success, string? error)>(new InvalidOperationException("Error BD")));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => controller.EliminarReserva(Guid.NewGuid().ToString(), request));
    }

    // ========================================================================
    // Helper
    // ========================================================================

    private static int GetStatusCode(IActionResult result)
    {
        return result switch
        {
            CreatedResult cr => cr.StatusCode ?? 201,
            CreatedAtActionResult car => car.StatusCode ?? 201,
            StatusCodeResult sc => sc.StatusCode,
            ObjectResult o => o.StatusCode ?? 200,
            _ => 200
        };
    }
}