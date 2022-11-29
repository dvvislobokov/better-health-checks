using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HealthChecks.Core
{
    public interface IHealthChecksMiddleware
    {
        Task InvokeAsync(HttpContext context);
    }
    
    public class HealthChecksMiddleware : IHealthChecksMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly IHealthCheckService _healthCheckService;

        public HealthChecksMiddleware(RequestDelegate requestDelegate, IHealthCheckService healthCheckService)
        {
            _requestDelegate = requestDelegate;
            _healthCheckService = healthCheckService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers.ContentType = "application/json";
            var res = await _healthCheckService.CheckHealth(CancellationToken.None);
            await context.Response.WriteAsJsonAsync(res);
        }
    }
}