using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Conduit.Infrastructure.Errors;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IStringLocalizer<ErrorHandlingMiddleware> _localizer;
    private static readonly Action<ILogger, string, Exception> LOGGER_MESSAGE =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            eventId: new EventId(id: 0, name: "ERROR"),
            formatString: "{Message}"
        );

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        IStringLocalizer<ErrorHandlingMiddleware> localizer,
        ILogger<ErrorHandlingMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, _logger, _localizer);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        ILogger<ErrorHandlingMiddleware> logger,
        IStringLocalizer<ErrorHandlingMiddleware> localizer
    )
    {
        string? result = null;
        switch (exception)
        {
            case RestException re:
                context.Response.StatusCode = (int)re.Code;
                result = JsonSerializer.Serialize(new { errors = re.Errors });
                break;
            case Exception e:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                LOGGER_MESSAGE(logger, "Unhandled Exception", e);
                result = JsonSerializer.Serialize(
                    new { errors = exception.Message }//localizer[Constants.InternalServerError].Value }
                );
                break;
            default:
                break;
        }

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(result ?? "{}");
    }
}
