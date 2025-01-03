using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using static Confluent.Kafka.ConfigPropertyNames;
namespace ChatAppBackE.Service
{
    public class ConsumerService : BackgroundService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IHubContext<ChatHub> _hubContext;

        public ConsumerService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "InventoryConsumerGroup",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("TestTopic");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(() => ProcessKafkaMessage(stoppingToken), stoppingToken);
                await Task.Delay(10, stoppingToken);
            }
            _consumer.Close();
        }
        public void ProcessKafkaMessage(CancellationToken stoppingToken)
        {
            try
            {
                var consumeResult = _consumer.Consume(stoppingToken);
                var message = consumeResult.Message.Value;
                _hubContext.Clients.All.SendAsync("ReceiveMessage", consumeResult.Message.Value);
                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString(), "Error processing Kafka message");
            }
        }

    }
}
