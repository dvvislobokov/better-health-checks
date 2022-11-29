using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BetterHealthChecks.Core.Models;

namespace BetterHealthChecks.Core
{
    public interface IHealthCheckService
    {
        Task<ICollection<HealthCheckResult>> CheckHealth(CancellationToken token);
        int GetStatusCode();
    }
    
    public class HealthCheckService : IHealthCheckService
    {
        private readonly List<IBetterHealthCheck> _healthChecks;
        private readonly IServiceProvider _serviceProvider;
        private readonly Func<ICollection<HealthCheckResult>, int>? _calculateStatusCode;
        private ICollection<HealthCheckResult> _checkResults;

        public HealthCheckService(IServiceProvider serviceProvider, Func<ICollection<HealthCheckResult>, int>? calculateStatusCode = null)
        {
            _serviceProvider = serviceProvider;
            _calculateStatusCode = calculateStatusCode;
            _healthChecks = new List<IBetterHealthCheck>();
            _checkResults = new List<HealthCheckResult>(_healthChecks.Count);
        }

        public async Task<ICollection<HealthCheckResult>> CheckHealth(CancellationToken token)
        {
            if (_healthChecks.Any())
                _checkResults = await Task.WhenAll(_healthChecks.Select(x => x.ExecuteAsync(token)));
            else
                _checkResults = new List<HealthCheckResult>();
            return _checkResults;
        }

        public int GetStatusCode()
        {
            if (_calculateStatusCode != null && _checkResults.Any())
                return _calculateStatusCode(_checkResults);
            return _checkResults.Any(x => x.Status == HealthStatus.Degrade || x.Status == HealthStatus.Unhealthy) ? 500 : 200;
        }

        public void AddHealthCheck(Func<IServiceProvider, IBetterHealthCheck> createHealthCheck)
        {
            _healthChecks.Add(createHealthCheck(_serviceProvider));
        }

        public void AddHealthCheck(IBetterHealthCheck betterHealthCheck)
        {
            _healthChecks.Add(betterHealthCheck);
        }
    }
}