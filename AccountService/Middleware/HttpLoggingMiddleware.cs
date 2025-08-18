using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace AccountService.Middleware;

public class HttpLoggingMiddleware(RequestDelegate next, ILogger<HttpLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        var requestBody = "";
        if (context.Request.ContentLength > 0)
        {
            using var reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        var stopwatch = Stopwatch.StartNew();
        var originalBody = context.Response.Body;

        await using var newBody = new MemoryStream();
        context.Response.Body = newBody;

        await next(context);

        stopwatch.Stop();

        newBody.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(newBody).ReadToEndAsync();
        newBody.Seek(0, SeekOrigin.Begin);

        logger.LogInformation(
            "HTTP {Method} {Path} CorrelationId={CorrelationId} " +
            "RequestHeaders={RequestHeaders} RequestBody={RequestBody} " +
            "ResponseCode={StatusCode} ResponseBody={ResponseBody} Duration={Elapsed}ms",
            context.Request.Method,
            context.Request.Path + context.Request.QueryString,
            correlationId,
            JsonSerializer.Serialize(context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())),
            requestBody,
            context.Response.StatusCode,
            responseBody,
            stopwatch.ElapsedMilliseconds
        );

        await newBody.CopyToAsync(originalBody);
    }
}
