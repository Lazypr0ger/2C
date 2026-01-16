using System.Diagnostics;
using System.Text;

namespace MiddleWareOcelot
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = Guid.NewGuid().ToString();
            context.Items["CorrelationId"] = correlationId;

            // Логируем входящий запрос
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("[{CorrelationId}] Incoming request: {Method} {Path}",
                correlationId, context.Request.Method, context.Request.Path);

            // Логируем заголовки (опционально)
            if (context.Request.Headers.Any())
            {
                _logger.LogDebug("[{CorrelationId}] Headers: {@Headers}",
                    correlationId, context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));
            }

            try
            {
                await _next(context);
                stopwatch.Stop();

                _logger.LogInformation("[{CorrelationId}] Response: {StatusCode} in {Elapsed}ms",
                    correlationId, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[{CorrelationId}] Error after {Elapsed}ms: {Message}",
                    correlationId, stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }
    }
}