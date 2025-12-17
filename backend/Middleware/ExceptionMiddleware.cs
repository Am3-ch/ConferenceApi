using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace ConferenceApi.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = context.TraceIdentifier;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}, Path: {Path}", traceId, context.Request.Path);
            await WriteProblemDetailsAsync(context, ex, traceId);
        }
    }

    private Task WriteProblemDetailsAsync(HttpContext context, Exception exception, string traceId)
    {
        (HttpStatusCode status, string title, string detail, bool isSensitive) = exception switch
        {
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized", "You are not authorized to perform this action.", false),
            InvalidOperationException => (HttpStatusCode.BadRequest, "InvalidOperation", exception.Message, false),
            ArgumentNullException => (HttpStatusCode.BadRequest, "ArgumentNull", exception.Message, false),
            ArgumentException => (HttpStatusCode.BadRequest, "BadRequest", exception.Message, false),
            KeyNotFoundException => (HttpStatusCode.NotFound, "NotFound", exception.Message, false),
            DbUpdateException dbEx => (HttpStatusCode.BadRequest, "DatabaseError", dbEx.InnerException?.Message ?? dbEx.Message, true),
            OperationCanceledException => (HttpStatusCode.RequestTimeout, "RequestCanceled", "The request was canceled.", false),
            _ => (HttpStatusCode.InternalServerError, "ServerError", "An unexpected error occurred.", true)
        };

        var clientDetail = _env.IsDevelopment()
            ? detail
            : status == HttpStatusCode.InternalServerError || isSensitive
                ? "An unexpected error occurred."
                : detail;

        var response = new
        {
            status = (int)status,
            error = title,
            message = clientDetail,
            traceId,
            path = context.Request.Path,
            method = context.Request.Method,
            user = context.User?.Identity?.IsAuthenticated == true ? context.User.Identity?.Name : null,
            exceptionType = _env.IsDevelopment() ? exception.GetType().Name : null
        };

        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

