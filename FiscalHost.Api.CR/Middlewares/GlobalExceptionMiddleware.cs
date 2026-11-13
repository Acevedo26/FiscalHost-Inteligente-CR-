using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace FiscalHost.Api.CR.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        
        var problemDetails = new ProblemDetails
        {
            Instance = context.Request.Path
        };

        if (exception is FiscalHost.Api.CR.Exceptions.BusinessRuleValidationException businessEx)
        {
            context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
            problemDetails.Status = context.Response.StatusCode;
            problemDetails.Title = "Error de validación de reglas de negocio.";
            problemDetails.Detail = businessEx.Message;
            if (businessEx.ErrorDetails != null)
            {
                problemDetails.Extensions["errors"] = businessEx.ErrorDetails;
            }
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            problemDetails.Status = context.Response.StatusCode;
            problemDetails.Title = "Un error inesperado ha ocurrido en el servidor.";
            problemDetails.Detail = exception.Message; // En entorno de producción se debería ocultar y cambiar por un mensaje genérico
        }

        var json = JsonSerializer.Serialize(problemDetails);
        return context.Response.WriteAsync(json);
    }
}
