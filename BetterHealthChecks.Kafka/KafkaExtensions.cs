using System;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace BetterHealthChecks.Kafka
{

    public static class KafkaExtensions
    {
        public static async Task<(bool IsSuccess, string Message)> TryCreateKafkaTopicsAsync
        (
            this IAdminClient adminClient,
            int partitionCount,
            params string[] topicNames
        )
        {
            if (topicNames == null || !topicNames.Any())
                return (false, "No topic names provided");

            var data = adminClient.GetMetadata(TimeSpan.FromSeconds(60));

            var topicSpecifications = topicNames
                .Where(tn => data.Topics.All(x => x.Topic != tn))
                .Select(tn => new TopicSpecification
                {
                    Name = tn,
                    NumPartitions = partitionCount
                }).ToArray();

            if (!topicSpecifications.Any()) return (true, "All topics have been already created");

            try
            {
                await adminClient.CreateTopicsAsync(topicSpecifications).ConfigureAwait(false);
            }
            catch (CreateTopicsException cte)
            {
                return (false,
                    string.Join(", ", cte.Results.Select(x => $"Topic {x.Topic} creation error: {x.Error.Reason}")));
            }

            return (true, $"Topics [{string.Join(", ", topicNames.Select(t => t))}] created successfully");
        }
    }
}