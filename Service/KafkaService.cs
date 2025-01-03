using Confluent.Kafka;

namespace ChatAppBackE.Service
{
    public class KafkaProviderService
    {
        private readonly IProducer<string, string> _producer;
        public KafkaProviderService(string server)
        {
            var config = new ProducerConfig { BootstrapServers = server };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }
        public async Task StartProducer(string topic, string key, string message)
        {
            var kafkamessage = new Message<string, string>
            {
                Key = key,
                Value = message
            };

            await _producer.ProduceAsync(topic, kafkamessage);
        }
    }
}
