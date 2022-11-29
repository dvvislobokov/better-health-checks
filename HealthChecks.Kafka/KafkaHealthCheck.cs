using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using HealthChecks.Core;
using HealthChecks.Core.Models;

namespace HealthChecks.Kafka
{
    public class KafkaHealthCheck : IHealthCheck, IDisposable
    {
        public string Name { get; set; }
        
        private readonly ProducerConfig _producerConfig = new ProducerConfig();
        private IProducer<string, string> _producer;
        private readonly string _topic;

        public KafkaHealthCheck(KafkaConfig config)
        {
            _topic = "health-check";
            _producerConfig.BootstrapServers = config.BootstrapServers;
            _producerConfig.SocketTimeoutMs = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
            _producerConfig.RequestTimeoutMs= (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
            _producerConfig.SocketMaxFails = 3;
            _producerConfig.MessageTimeoutMs = 5000;
        }


        public async Task<HealthCheckResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                _producer = new ProducerBuilder<string, string>(_producerConfig).Build();
                
                var message = new Message<string, string>
                {
                    Key = "healthcheck-key",
                    Value = $"Check Kafka healthy on {DateTime.UtcNow}"
                };

                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var result = await _producer.ProduceAsync(_topic, message, cancellationToken);
                stopWatch.Stop();

                if (result.Status == PersistenceStatus.NotPersisted)
                {
                    return new HealthCheckResult()
                    {
                        Duration = stopWatch.Elapsed,
                        Name = Name,
                        Exception = "Message is not persisted or a failure is raised on health check for kafka.",
                        Status = HealthStatus.Unhealthy
                    };
                }

                return new HealthCheckResult()
                {
                    Duration = stopWatch.Elapsed,
                    Name = Name,
                    Status = HealthStatus.Unhealthy
                };
            }
            catch (Exception e)
            {
                return new HealthCheckResult()
                {
                    Name = Name,
                    Exception = e.Message,
                    Status = HealthStatus.Unhealthy
                };
            }
        }

        public void Dispose()
        {
            if(_producer!=null)
                _producer.Dispose();
        }
    }
}