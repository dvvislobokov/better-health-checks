using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace HealthChecks.Core
{
    public static class Extensions
    {
        public static IEndpointConventionBuilder MapBetterHealthChecks(this IEndpointRouteBuilder builder, string pattern="/health")
        {
            return builder.Map("/health", builder.CreateApplicationBuilder()
                .UseMiddleware<HealthChecksMiddleware>()
                .Build());
        }
        
        public static void AddBetterHealthChecks(this IServiceCollection services, Func<List<IHealthCheck>, List<IHealthCheck>> addHealthChecks)
        {
            services.AddSingleton<IHealthCheckService, HealthCheckService>(x =>
            {
                var healthCheckService = new HealthCheckService(x);
                var healthChecks = addHealthChecks(new List<IHealthCheck>());
                foreach (var healthCheck in healthChecks)
                {
                    healthCheckService.AddHealthCheck(healthCheck);
                }
                return healthCheckService;
            });
        }
    }
}