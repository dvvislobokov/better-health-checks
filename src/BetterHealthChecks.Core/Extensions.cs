using System;
using System.Collections.Generic;
using BetterHealthChecks.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BetterHealthChecks.Core
{
    public static class Extensions
    {
        public static IEndpointConventionBuilder MapBetterHealthChecks(this IEndpointRouteBuilder builder, string pattern="/health")
        {
            return builder.Map("/health", builder.CreateApplicationBuilder()
                .UseMiddleware<BetterHealthChecksMiddleware>()
                .Build());
        }
        
        public static void AddBetterHealthChecks(this IServiceCollection services, Func<List<IBetterHealthCheck>, List<IBetterHealthCheck>> addHealthChecks, Func<ICollection<HealthCheckResult>,int>? calculateStatusCode = null)
        {
            services.AddSingleton<IHealthCheckService, HealthCheckService>(x =>
            {
                var healthCheckService = new HealthCheckService(x, calculateStatusCode);
                var healthChecks = addHealthChecks(new List<IBetterHealthCheck>());
                foreach (var healthCheck in healthChecks)
                {
                    healthCheckService.AddHealthCheck(healthCheck);
                }
                return healthCheckService;
            });
        }
    }
}