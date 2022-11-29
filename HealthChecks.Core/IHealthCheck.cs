using System.Threading;
using System.Threading.Tasks;
using HealthChecks.Core.Models;

namespace HealthChecks.Core
{
    public interface IHealthCheck
    {
        public string Name { get; set; }
        Task<HealthCheckResult> ExecuteAsync(CancellationToken cancellationToken);
    }
}