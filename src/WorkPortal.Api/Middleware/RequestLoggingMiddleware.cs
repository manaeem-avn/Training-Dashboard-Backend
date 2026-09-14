using System.Diagnostics;

namespace WorkPortal.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<RequestLoggingMiddleware> logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        string correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
            ?? Guid.NewGuid().ToString("N")[..12];

        context.Response.Headers["X-Correlation-Id"] = correlationId;

        using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            Stopwatch watch = Stopwatch.StartNew();
            await next(context);
            watch.Stop();

            logger.LogInformation("{Method} {Path} by {User} returned {Status} in {Elapsed} ms",
                context.Request.Method,
                context.Request.Path,
                context.User.Identity?.Name ?? "anonymous",
                context.Response.StatusCode,
                watch.ElapsedMilliseconds);
        }
    }
}
