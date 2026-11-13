using System.Security.Claims;
using FiscalHost.Api.CR.Controllers;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Requests;
using FiscalHost.Api.CR.Models.DTOs.TaxIntelligence.Responses;
using FiscalHost.Api.CR.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FiscalHost.Tests.Unit;

public class GeneradorBorradorControllerTests
{
    [Fact]
    public async Task GetBorradorD104_UsesAuthenticatedUserInsteadOfRequestUserId()
    {
        var authenticatedUserId = Guid.NewGuid();
        var requestUserId = Guid.NewGuid();
        var serviceMock = new Mock<IGeneradorBorradorService>();
        var expected = new BorradorD104Dto();

        serviceMock
            .Setup(service => service.GenerarD104Async(authenticatedUserId, 2026, 1))
            .ReturnsAsync(expected);

        var controller = CreateController(serviceMock, authenticatedUserId);

        var result = await controller.GetBorradorD104(new GenerarBorradorD104Request
        {
            UsuarioId = requestUserId,
            Anio = 2026,
            Mes = 1
        });

        Assert.IsType<OkObjectResult>(result.Result);
        serviceMock.Verify(
            service => service.GenerarD104Async(authenticatedUserId, 2026, 1),
            Times.Once);
        serviceMock.Verify(
            service => service.GenerarD104Async(requestUserId, 2026, 1),
            Times.Never);
    }

    [Fact]
    public async Task GetBorradorD125_UsesAuthenticatedUserInsteadOfRequestUserId()
    {
        var authenticatedUserId = Guid.NewGuid();
        var requestUserId = Guid.NewGuid();
        var serviceMock = new Mock<IGeneradorBorradorService>();
        var expected = new BorradorD125Dto();

        serviceMock
            .Setup(service => service.GenerarD125Async(authenticatedUserId, 2026, true))
            .ReturnsAsync(expected);

        var controller = CreateController(serviceMock, authenticatedUserId);

        var result = await controller.GetBorradorD125(new GenerarBorradorD125Request
        {
            UsuarioId = requestUserId,
            Anio = 2026,
            RegimenUtilidades = true
        });

        Assert.IsType<OkObjectResult>(result.Result);
        serviceMock.Verify(
            service => service.GenerarD125Async(authenticatedUserId, 2026, true),
            Times.Once);
        serviceMock.Verify(
            service => service.GenerarD125Async(requestUserId, 2026, true),
            Times.Never);
    }

    [Fact]
    public async Task GetBorradorD104_WhenIdentityIsMissing_ReturnsUnauthorizedWithoutCallingService()
    {
        var serviceMock = new Mock<IGeneradorBorradorService>();
        var controller = CreateControllerWithClaim(serviceMock, null);

        var result = await controller.GetBorradorD104(new GenerarBorradorD104Request
        {
            UsuarioId = Guid.NewGuid(),
            Anio = 2026,
            Mes = 1
        });

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
        serviceMock.Verify(
            service => service.GenerarD104Async(It.IsAny<Guid>(), It.IsAny<short>(), It.IsAny<short>()),
            Times.Never);
    }

    [Fact]
    public async Task GetBorradorD104_WhenIdentityIsInvalid_ReturnsUnauthorizedWithoutCallingService()
    {
        var serviceMock = new Mock<IGeneradorBorradorService>();
        var controller = CreateControllerWithClaim(serviceMock, "not-a-guid");

        var result = await controller.GetBorradorD104(new GenerarBorradorD104Request
        {
            UsuarioId = Guid.NewGuid(),
            Anio = 2026,
            Mes = 1
        });

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
        serviceMock.Verify(
            service => service.GenerarD104Async(It.IsAny<Guid>(), It.IsAny<short>(), It.IsAny<short>()),
            Times.Never);
    }

    [Fact]
    public async Task GetBorradorD125_WhenIdentityIsInvalid_ReturnsUnauthorizedWithoutCallingService()
    {
        var serviceMock = new Mock<IGeneradorBorradorService>();
        var controller = CreateControllerWithClaim(serviceMock, "not-a-guid");

        var result = await controller.GetBorradorD125(new GenerarBorradorD125Request
        {
            UsuarioId = Guid.NewGuid(),
            Anio = 2026,
            RegimenUtilidades = true
        });

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
        serviceMock.Verify(
            service => service.GenerarD125Async(It.IsAny<Guid>(), It.IsAny<short>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public async Task GetBorradorD125_WhenIdentityIsMissing_ReturnsUnauthorizedWithoutCallingService()
    {
        var serviceMock = new Mock<IGeneradorBorradorService>();
        var controller = CreateControllerWithClaim(serviceMock, null);

        var result = await controller.GetBorradorD125(new GenerarBorradorD125Request
        {
            UsuarioId = Guid.NewGuid(),
            Anio = 2026,
            RegimenUtilidades = true
        });

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
        serviceMock.Verify(
            service => service.GenerarD125Async(It.IsAny<Guid>(), It.IsAny<short>(), It.IsAny<bool>()),
            Times.Never);
    }

    private static GeneradorBorradorController CreateController(
        Mock<IGeneradorBorradorService> serviceMock,
        Guid authenticatedUserId)
    {
        return CreateControllerWithClaim(serviceMock, authenticatedUserId.ToString());
    }

    private static GeneradorBorradorController CreateControllerWithClaim(
        Mock<IGeneradorBorradorService> serviceMock,
        string? claimValue)
    {
        var identity = new ClaimsIdentity(
            claimValue is null
                ? Array.Empty<Claim>()
                : new[] { new Claim(ClaimTypes.NameIdentifier, claimValue) },
            authenticationType: "Test");

        return new GeneradorBorradorController(serviceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            }
        };
    }
}
