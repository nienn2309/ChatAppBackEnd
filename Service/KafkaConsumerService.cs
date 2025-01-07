using ChatAppBackE;
using ChatAppBackE.Models;
using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace ChatAppBackE.Service 
{
    public class KafkaConsumerService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ConsumerConfig _consumerConfig;

        public KafkaConsumerService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;

            // Configure Kafka consumer
            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "signalr-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
        }

        public void StartConsuming(string topic)
        {
            Task.Run(() => ConsumeMessagesAsync(topic));
        }

        private async Task ConsumeMessagesAsync(string topic)
        {
            using (var consumer = new ConsumerBuilder<string, string>(_consumerConfig).Build())
            {
                consumer.Subscribe(topic);

                try
                {
                    while (true)
                    {
                        // Consume a message
                        var consumeResult = consumer.Consume(CancellationToken.None);

                        var conversation_id = consumeResult.Message.Key;
                        var message = consumeResult.Message.Value;
                        // Send message to SignalR clients
                        await _hubContext.Clients.Group(conversation_id).SendAsync("ReceiveMessage", message);
                        Console.WriteLine($"Message '{message}' sent to conversation '{conversation_id}'");
                    }
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occurred: {e.Error.Reason}");
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
    }
}
