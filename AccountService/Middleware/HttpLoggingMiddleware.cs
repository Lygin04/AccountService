using System.Diagnostics;

namespace AccountService.Middleware;

public class HttpLoggingMiddleware(RequestDelegate next, ILogger<HttpLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Response.Headers["X-Correlation-Id"].FirstOrDefault();
        var stopwatch = Stopwatch.StartNew();

        await next(context);

        stopwatch.Stop();

        logger.LogInformation(
            "HTTP {Method} {Path} CorrelationId={CorrelationId} => {StatusCode} ({Elapsed} ms)",
            context.Request.Method,
            context.Request.Path + context.Request.QueryString,
            correlationId,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds
        );
    }
}

