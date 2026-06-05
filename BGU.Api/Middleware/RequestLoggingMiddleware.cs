using System.Diagnostics;
using System.Security.Claims;

namespace BGU.Api.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    private const int SlowRequestThresholdMs = 1000;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds;
            var statusCode = context.Response.StatusCode;
            var logLevel = GetLogLevel(statusCode, elapsedMs);

            logger.Log(
                logLevel,
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms. RequestId: {RequestId}. User: {UserId}. ClientIp: {ClientIp}",
                context.Request.Method,
                context.Request.Path.Value,
                statusCode,
                elapsedMs,
                context.TraceIdentifier,
                GetUserId(context),
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown"
            );
        }
    }

    private static LogLevel GetLogLevel(int statusCode, long elapsedMs)
    {
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            return LogLevel.Error;
        }

        if (statusCode >= StatusCodes.Status400BadRequest || elapsedMs >= SlowRequestThresholdMs)
        {
            return LogLevel.Warning;
        }

        return LogLevel.Information;
    }

    private static string GetUserId(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            return "anonymous";
        }

        return context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue("sub")
            ?? context.User.Identity?.Name
            ?? "authenticated";
    }
}
