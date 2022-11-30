namespace BetterHealthChecks.Kafka
{
    public class KafkaConfig
    {
        public string BootstrapServers { get; set; }

        public KafkaConfig()
        {
            
        }

        public KafkaConfig(string bootstrapServers)
        {
            BootstrapServers = bootstrapServers;
        }
    }
}