using System;
using System.Threading;
using System.Threading.Tasks;
using BetterHealthChecks.Core;
using BetterHealthChecks.Core.Models;
using Npgsql;

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

        public async Task<HealthCheckResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            using var duration = new Duration();
            try
            {
                await using var sqlConnection = new NpgsqlConnection(_connectionString);
                await sqlConnection.OpenAsync(cancellationToken);
                await using var command = sqlConnection.CreateCommand();
                command.CommandText = string.IsNullOrEmpty(_customQuery) ? "SELECT 1;" : _customQuery;
                command.CommandTimeout = _timeout == TimeSpan.Zero ? 30 : (int)_timeout.TotalSeconds;
                await command.ExecuteNonQueryAsync(cancellationToken);
                return new HealthCheckResult()
                {
                    Name = Name,
                    Duration = duration.GetDuration(),
                    Status = HealthStatus.Health
                };
            }
            catch (Exception e)
            {
                return new HealthCheckResult()
                {
                    Name = Name,
                    Duration = duration.GetDuration(),
                    Exception = e.Message,
                    Status = HealthStatus.Unhealthy
                };
            }
        }

        public void Dispose()
        {
        }
    }
}