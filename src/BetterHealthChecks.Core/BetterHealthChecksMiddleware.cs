using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BetterHealthChecks.Core
{

    public interface IBetterHealthChecksMiddleware
    {
        Task InvokeAsync(HttpContext context);
    }
    
    public class BetterHealthChecksMiddleware : IBetterHealthChecksMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly IHealthCheckService _healthCheckService;

        public BetterHealthChecksMiddleware(RequestDelegate requestDelegate, IHealthCheckService healthCheckService)
        {
            _requestDelegate = requestDelegate;
            _healthCheckService = healthCheckService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers.Add("ContentType", "application/json");
            var res = await _healthCheckService.CheckHealth(CancellationToken.None);
            context.Response.StatusCode = _healthCheckService.GetStatusCode();
            await context.Response.WriteAsync(JsonSerializer.Serialize(res, new JsonSerializerOptions()
            {
                Converters = { new JsonStringEnumConverter() }
            }));
        }
    }
}