using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BetterHealthChecks.Core;
using BetterHealthChecks.Core.Models;

namespace HealthChecks.Http
{
    public class HttpConfig
    {
        public string Host { get; set; }
        public HttpMethod Method { get; set; }
        public string? QueryString { get; set; }
        public string? Body { get; set; }
        public TimeSpan? Timeout { get; set; }

        public HttpConfig(string host, HttpMethod method, TimeSpan? timeout, string? queryString = null , string? body = null)
        {
            Host = host;
            Method = method;
            QueryString = queryString;
            Body = body;
            Timeout = timeout;
        }

        public HttpConfig()
        {
            Host = "http://localhost";
            Method = HttpMethod.Get;
        }
    }

    public class HttpHealthCheck : IBetterHealthCheck
    {
        public string Name { get; set; }

        private readonly HttpConfig _config;
        private readonly bool _checkSsl;
        private readonly int[]? _healthStatusCodes;
        private readonly int[]? _degradeStatusCodes;
        private readonly int[]? _unhealthyStatusCodes;

        public HttpHealthCheck(string name, HttpConfig config, bool checkSsl = true, int[]? healthStatusCodes = null,
            int[]? degradeStatusCodes = null, int[]? unhealthyStatusCodes = null)
        {
            Name = name;
            _config = config;
            _checkSsl = checkSsl;
            _healthStatusCodes = healthStatusCodes;
            _degradeStatusCodes = degradeStatusCodes;
            _unhealthyStatusCodes = unhealthyStatusCodes;
        }


        public async Task<HealthCheckResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback =
                    _checkSsl ? null : (_, _, _, _) => true
            });
            using var duration = new Duration();
            try
            {
                httpClient.Timeout = _config.Timeout ?? TimeSpan.FromMinutes(5);
                var message = new HttpRequestMessage(_config.Method, _config.Host);
                if (!string.IsNullOrEmpty(_config.Body) && _config.Method == HttpMethod.Post)
                    message.Content = new StringContent(_config.Body);
                
                var res = await httpClient.SendAsync(message, cancellationToken);
                var time = duration.GetDuration();
                if (_healthStatusCodes != null && _healthStatusCodes.Contains((int)res.StatusCode))
                    return new HealthCheckResult() { Name = Name, Status = HealthStatus.Health, Duration = time};
                if (_degradeStatusCodes != null && _degradeStatusCodes.Contains((int)res.StatusCode))
                    return new HealthCheckResult() { Name = Name, Status = HealthStatus.Degrade, Duration = time};
                if (_unhealthyStatusCodes != null && _unhealthyStatusCodes.Contains((int)res.StatusCode))
                    return new HealthCheckResult() { Name = Name, Status = HealthStatus.Unhealthy, Duration = time, Exception = await res.Content.ReadAsStringAsync(cancellationToken)};
                
                if ((int)res.StatusCode == 200)
                    return new HealthCheckResult() { Name = Name, Status = HealthStatus.Health, Duration = time };
                
                return new HealthCheckResult()
                {
                    Name = Name, 
                    Status = HealthStatus.Unhealthy,
                    Duration = time,
                    Exception = await res.Content.ReadAsStringAsync(CancellationToken.None)
                };
            }
            catch (Exception e)
            {
                return new HealthCheckResult()
                {
                    Name = Name,
                    Status = HealthStatus.Unhealthy,
                    Exception = e.Message,
                    Duration = duration.GetDuration()
                };
            }
        }
    }
}