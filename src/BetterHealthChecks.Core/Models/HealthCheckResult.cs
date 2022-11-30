using System;

namespace BetterHealthChecks.Core.Models
{
    public enum HealthStatus
    {
        Health,
        Unhealthy,
        Degrade
    }
    
    public class HealthCheckResult
    {
        public string Name { get; set; }
        public HealthStatus Status { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Exception { get; set; }
    }
}