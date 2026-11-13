using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FiscalHost.Api.CR.Controllers;
using FiscalHost.Api.CR.Models.DTOs.Operations.Requests;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;
using FiscalHost.Api.CR.Models.Entities.Audit;
using FiscalHost.Api.CR.Models.Entities.Operations;
using FiscalHost.Api.CR.Models.Enums.Audit;
using FiscalHost.Api.CR.Repositories;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace FiscalHost.Tests.Unit;

public class ChallengerAuditAndEndpointsStressTests
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
    public async Task ActualizarReserva_SinRazonCambio_FallaPorLey8968_SinPersistirAuditoria()
    {
        // Arrange
        var repository = Substitute.For<IOperacionManualRepository>();
        var sut = new OperacionManualService(
            repository,
            Substitute.For<IBlobStorageService>(),
            Substitute.For<IOcrService>()
        );

        var request = new UpdateReservaRequest
        {
            Monto = 75000,
            Huesped = "Cliente Modificado",
            RazonCambio = "" // Ausente
        };

        // Act
        var (success, error) = await sut.ActualizarReservaAsync(Guid.NewGuid().ToString(), request);

        // Assert
        Assert.False(success);
        Assert.NotNull(error);
        Assert.Contains("Ley 8968", error, StringComparison.OrdinalIgnoreCase);
        await repository.DidNotReceive().AddAuditoriaAsync(Arg.Any<AuditoriaOperacion>());
        await repository.DidNotReceive().UpdateReservaAsync(Arg.Any<Reserva>());
        await repository.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task ActualizarReserva_ConRazonCambio_RegistraAuditoriaInalterableConValoresPreviosYNuevos()
    {
        // Arrange
        var repository = Substitute.For<IOperacionManualRepository>();
        var sut = new OperacionManualService(
            repository,
            Substitute.For<IBlobStorageService>(),
            Substitute.For<IOcrService>()
        );

        var reservaId = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        var existingReserva = new Reserva
        {
            ReservaId = reservaId,
            UsuarioId = usuarioId,
            MontoColones = 50000,
            MontoBruto = 50000,
            NombreHuesped = "Huésped Original",
            FechaInicio = new DateTime(2026, 4, 1)
        };

        repository.GetReservaByIdAsync(reservaId).Returns(existingReserva);

        var request = new UpdateReservaRequest
        {
            Monto = 65000,
            Huesped = "Huésped Actualizado",
            RazonCambio = "Corrección de tarifa acordada por correo bajo Ley 8968"
        };

        AuditoriaOperacion? capturedAudit = null;
        await repository.AddAuditoriaAsync(Arg.Do<AuditoriaOperacion>(a => capturedAudit = a));

        // Act
        var (success, error) = await sut.ActualizarReservaAsync(reservaId.ToString(), request);

        // Assert
        Assert.True(success);
        Assert.Null(error);

        Assert.NotNull(capturedAudit);
        Assert.Equal(usuarioId, capturedAudit.UsuarioId);
        Assert.Equal(OperacionAuditoria.UPDATE, capturedAudit.Operacion);
        Assert.Equal("reserva", capturedAudit.TablaAfectada);
        Assert.Equal(reservaId, capturedAudit.RegistroId);
        Assert.Equal("Corrección de tarifa acordada por correo bajo Ley 8968", capturedAudit.Justificacion);
        Assert.True(capturedAudit.OldValues.Contains("Huésped Original") || capturedAudit.OldValues.Contains(@"Hu\u00E9sped Original"));
        Assert.True(capturedAudit.NewValues.Contains("Huésped Actualizado") || capturedAudit.NewValues.Contains(@"Hu\u00E9sped Actualizado"));

        await repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task EliminarReserva_SinRazonCambio_FallaPorLey8968_SinPersistirAuditoria()
    {
        // Arrange
        var repository = Substitute.For<IOperacionManualRepository>();
        var sut = new OperacionManualService(
            repository,
            Substitute.For<IBlobStorageService>(),
            Substitute.For<IOcrService>()
        );

        var request = new DeleteReservaRequest
        {
            RazonCambio = null
        };

        // Act
        var (success, error) = await sut.EliminarReservaAsync(Guid.NewGuid().ToString(), request);

        // Assert
        Assert.False(success);
        Assert.NotNull(error);
        Assert.Contains("Ley 8968", error, StringComparison.OrdinalIgnoreCase);
        await repository.DidNotReceive().AddAuditoriaAsync(Arg.Any<AuditoriaOperacion>());
        await repository.DidNotReceive().DeleteReservaAsync(Arg.Any<Reserva>());
        await repository.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task EliminarReserva_ConRazonCambio_RegistraAuditoriaInalterableDeEliminacion()
    {
        // Arrange
        var repository = Substitute.For<IOperacionManualRepository>();
        var sut = new OperacionManualService(
            repository,
            Substitute.For<IBlobStorageService>(),
            Substitute.For<IOcrService>()
        );

        var reservaId = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        var existingReserva = new Reserva
        {
            ReservaId = reservaId,
            UsuarioId = usuarioId,
            MontoColones = 90000,
            NombreHuesped = "Cliente Cancelado",
            FechaInicio = new DateTime(2026, 5, 10)
        };

        repository.GetReservaByIdAsync(reservaId).Returns(existingReserva);

        var request = new DeleteReservaRequest
        {
            RazonCambio = "Cancelación definitiva por fuerza mayor comprobada"
        };

        // Act
        var (success, error) = await sut.EliminarReservaAsync(reservaId.ToString(), request);

        // Assert
        Assert.True(success);
        Assert.Null(error);

        await repository.Received(1).DeleteReservaAsync(Arg.Is<Reserva>(r => r.ReservaId == reservaId));

        await repository.Received(1).AddAuditoriaAsync(Arg.Is<AuditoriaOperacion>(a =>
            a.UsuarioId == usuarioId &&
            a.Operacion == OperacionAuditoria.DELETE &&
            a.TablaAfectada == "reserva" &&
            a.RegistroId == reservaId &&
            a.Justificacion == "Cancelación definitiva por fuerza mayor comprobada" &&
            a.OldValues != null && a.OldValues.Contains("Cliente Cancelado")
        ));

        await repository.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task SimuladorFiscal_CrearSimulacion_Retorna201Created()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var simulacionId = Guid.NewGuid();
        var serviceMock = Substitute.For<ISimulacionFiscalService>();

        var request = new CreateSimulacionFiscalRequest
        {
            Nombre = "Escenario Alta Temporada 2026",
            PeriodoBaseAnio = 2026,
            Parametros = new SimulacionParametrosDto()
        };

        var responseDto = new SimulacionFiscalResponseDto
        {
            SimulacionId = simulacionId,
            Nombre = "Escenario Alta Temporada 2026",
            PeriodoBaseAnio = 2026
        };

        serviceMock.CrearSimulacionAsync(usuarioId, request).Returns(responseDto);

        var controller = new SimuladorFiscalController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(usuarioId)
        };

        // Act
        var result = await controller.CrearSimulacion(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        var body = Assert.IsType<SimulacionFiscalResponseDto>(createdResult.Value);
        Assert.Equal(simulacionId, body.SimulacionId);
    }

    [Fact]
    public async Task ObligacionesTributarias_ConsultarDeudaInexistente_Retorna404NotFound()
    {
        // Arrange
        var serviceMock = Substitute.For<IObligacionTributariaService>();
        var controller = new ObligacionesTributariasController(serviceMock)
        {
            ControllerContext = CreateContextWithUser(Guid.NewGuid())
        };

        serviceMock.ConsultarDeudaAsync(Arg.Any<Guid>()).Returns((ObligacionTributariaDto?)null);

        // Act
        var result = await controller.ConsultarDeuda(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
