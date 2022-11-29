using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthChecks.Core.Models;

namespace HealthChecks.Core
{
    public interface IHealthCheckService
    {
        Task<ICollection<HealthCheckResult>> CheckHealth(CancellationToken token);
        void AddHealthCheck(Func<IServiceProvider, IHealthCheck> createHealthCheck);
        void AddHealthCheck(IHealthCheck healthCheck);
    }
    
    public class HealthCheckService : IHealthCheckService
    {
        private readonly List<IHealthCheck> _healthChecks;
        private readonly IServiceProvider _serviceProvider;

        public HealthCheckService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _healthChecks = new List<IHealthCheck>();
        }

        public async Task<ICollection<HealthCheckResult>> CheckHealth(CancellationToken token)
        {
            return await Task.WhenAll(_healthChecks.Select(x => x.ExecuteAsync(token)));
        }

        public void AddHealthCheck(Func<IServiceProvider, IHealthCheck> createHealthCheck)
        {
            _healthChecks.Add(createHealthCheck(_serviceProvider));
        }

        public void AddHealthCheck(IHealthCheck healthCheck)
        {
            _healthChecks.Add(healthCheck);
        }
    }
}