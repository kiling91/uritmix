using System.Net;
using FluentValidation;
using Helpers.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Helpers.WebApi.ErrorHandling;

public class ErrorHandlingMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, _logger);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        ILogger<ErrorHandlingMiddleware> logger)
    {
        switch (exception)
        {
            case RestException ex:
                context.Response.StatusCode = (int)ex.Code;
                await context.Response.WriteAsJsonAsync(new ErrorResponse(
                    ReasonPhrases.GetReasonPhrase(context.Response.StatusCode),
                    ex.Errors
                ));
                break;
            case ValidationException ex:
                context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                var errors = new List<PropertyError>();
                foreach (var exError in ex.Errors)
                    errors.Add(new PropertyError(exError.PropertyName, exError.ErrorMessage));
                await context.Response.WriteAsJsonAsync(new ValidError(errors));
                break;
            case { } ex:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    ExceptionType = exception.GetType().Name,
                    ex.Message,
                    StackTrace = ex.StackTrace ?? ""
                });
                logger.LogError(ex, "InternalServerError");
                break;
        }
    }
}