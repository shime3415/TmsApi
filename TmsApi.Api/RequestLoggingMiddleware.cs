
using System.Diagnostics;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Create correlation ID
        var correlationId = Guid.NewGuid().ToString("N")[..8];

        // 2. Attach to response early
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        // 3. Start timer
        var stopwatch = Stopwatch.StartNew();

        // 4. Log request start
        _logger.LogInformation(
            "[START] {Method} {Path} | ID={Id}",
            context.Request.Method,
            context.Request.Path,
            correlationId);

        // 5. Pass to next middleware
        await _next(context);

        // 6. Stop timer
        stopwatch.Stop();

        // 7. Log request end
        _logger.LogInformation(
            "[END] {Method} {Path} | Status={StatusCode} | Time={Time}ms | ID={Id}",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds,
            correlationId);
    }
}