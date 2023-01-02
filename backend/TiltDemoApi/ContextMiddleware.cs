using OpenTelemetry;

namespace TiltDemoApi;

public class ContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ContextMiddleware> _logger;

    public ContextMiddleware(RequestDelegate next, ILogger<ContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var scopes = new List<KeyValuePair<string, object>>
        {
            new KeyValuePair<string, object>("clientId", "test12"),
        };
        Baggage.SetBaggage("sessionid", "someid");
        using (var _ = _logger.BeginScope(scopes))
        {
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }
}