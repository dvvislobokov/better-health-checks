using System.Threading;
using System.Threading.Tasks;
using BetterHealthChecks.Core.Models;

namespace BetterHealthChecks.Core
{
    public interface IBetterHealthCheck
    {
        public string Name { get; set; }
        Task<HealthCheckResult> ExecuteAsync(CancellationToken cancellationToken);
    }
}