using System;
using System.Threading;
using System.Threading.Tasks;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Client;
using BetterHealthChecks.Core;
using BetterHealthChecks.Core.Models;

namespace BetterHealthChecks.Ignite
{
    public class IgniteHealthCheck : IBetterHealthCheck
    {
        public IgniteHealthCheck(string name, string host)
        {
            _host = host;
            Name = name;
        }

        public string Name { get; set; }
        private readonly string _host;

        public async Task<HealthCheckResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            using var duration = new Duration();
            try
            {
                var cgf = new IgniteClientConfiguration(_host);
                using var client = Ignition.StartClient(cgf);
                var cache = client.GetOrCreateCache<string, string>("health-checks");
                var key = Guid.NewGuid().ToString("N");
                var value = Guid.NewGuid().ToString("N");
                await cache.PutAsync(key, value);
                await cache.GetAsync(key);
                client.DestroyCache("health-checks");
                return new HealthCheckResult()
                {
                    Name = Name,
                    Status = HealthStatus.Health,
                    Duration = duration.GetDuration(),
                };
            }
            catch (Exception e)
            {
                return new HealthCheckResult()
                {
                    Name = Name,
                    Status = HealthStatus.Unhealthy,
                    Duration = duration.GetDuration(),
                    Exception = e.Message
                };
            }
        }
    }
}