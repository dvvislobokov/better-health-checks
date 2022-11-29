using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using BetterHealthChecks.Core;
using BetterHealthChecks.Core.Models;
using Confluent.Kafka.Admin;

namespace BetterHealthChecks.Kafka
{
    public class KafkaHealthCheck : IBetterHealthCheck, IDisposable
    {
        public string Name { get; set; }

        private readonly ProducerConfig _producerConfig = new ProducerConfig();
        private IProducer<string, string> _producer;
        private readonly string _topic;
        private readonly IAdminClient _adminClient;

        public KafkaHealthCheck(KafkaConfig config, string name)
        {
            Name = name;
            _topic = "health-check";
            _producerConfig.BootstrapServers = config.BootstrapServers;
            _producerConfig.SocketTimeoutMs = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
            _producerConfig.RequestTimeoutMs = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
            _producerConfig.SocketMaxFails = 3;
            _producerConfig.MessageTimeoutMs = 5000;
        }


        public async Task<HealthCheckResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig {BootstrapServers = _producerConfig.BootstrapServers}).Build();
            try
            {
                await adminClient.CreateTopicsAsync(new[] { 
                    new TopicSpecification { Name = _topic, ReplicationFactor = 1, NumPartitions = 1 } });
            }
            catch (CreateTopicsException e)
            {
                Console.WriteLine($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
            }

            using var producer = new ProducerBuilder<string, string>(_producerConfig).Build();
            try
            {
                var message = new Message<string, string>
                {
                    Key = "healthcheck-key",
                    Value = $"Check Kafka healthy on {DateTime.UtcNow}"
                };

                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var result = await producer.ProduceAsync(_topic, message, cancellationToken);
                stopWatch.Stop();

                if (result.Status == PersistenceStatus.NotPersisted)
                {
                    return new HealthCheckResult()
                    {
                        Duration = stopWatch.Elapsed,
                        Name = Name,
                        Exception =
                            "Message is not persisted or a failure is raised on health check for kafka.",
                        Status = HealthStatus.Unhealthy
                    };
                }

                return new HealthCheckResult()
                {
                    Duration = stopWatch.Elapsed,
                    Name = Name,
                    Status = HealthStatus.Health
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
            if (_producer != null)
                _producer.Dispose();
        }
    }
}