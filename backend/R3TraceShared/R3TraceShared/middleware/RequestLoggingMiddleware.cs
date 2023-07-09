using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace R3TraceShared.middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    protected readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        // Log the request details
        _logger.LogInformation($"Request {context.Request.Method}: {context.Request.Path}");

        // Log request body if needed
        if (context.Request.ContentLength.HasValue && context.Request.ContentLength > 0)
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var requestBody = await reader.ReadToEndAsync();
            _logger.LogInformation($"Request Body: {requestBody}");
            context.Request.Body.Position = 0; // Reset the request body stream position
        }

        // Call the next middleware
        await _next(context);
    }
}