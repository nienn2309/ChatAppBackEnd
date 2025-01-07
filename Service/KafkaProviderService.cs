using Confluent.Kafka;
using System;
using System.Threading.Tasks;

namespace ChatAppBackE.Service
{
    public class KafkaProducer
    {
        private readonly IProducer<string, string> _producer;

        public KafkaProducer()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092"
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task SendMessageAsync(string key, string message, string topic)
        {
            try
            {
                await _producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = message });
                Console.WriteLine($"Message '{message}' sent to Kafka topic '{topic}'");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Error producing message: {e.Error.Reason}");
            }
        }
    }

}