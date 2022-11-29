using System;
using System.Threading;
using System.Threading.Tasks;
using BetterHealthChecks.Core;
using BetterHealthChecks.Core.Models;

namespace BetterHealthChecks.PostgreSQL
{
    public class PostgresHealthCheck  : IBetterHealthCheck, IDisposable
    {
        public string Name { get; set; }

        private readonly string _connectionString;
        private readonly string _customQuery;
        private readonly TimeSpan _timeout;

        public PostgresHealthCheck( string connectionString,TimeSpan timeout, string customQuery, string name = "Postgres")
        {
            _timeout = timeout;
            _customQuery = customQuery;
            Name = name;
            _connectionString = connectionString;
        }

        public Task<HealthCheckResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}