using KwetterShared.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TimelineService.Messaging
{
    public static class RabbitSubscriber
    {
        public static Kweet LastKweet { get; private set; }
        public static void OpenChannel()
        {
            Console.WriteLine("Opening channel...");
            string host = Environment.GetEnvironmentVariable("RabbitHost");
            var factory = new ConnectionFactory() { HostName = host };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "kweets",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    byte[] body = ea.Body.ToArray();
                    string message = Encoding.UTF8.GetString(body);

                    LastKweet = JsonSerializer.Deserialize<Kweet>(message);

                    Console.WriteLine(" [x] Received {0}", message);
                };
                channel.BasicConsume(queue: "kweets",
                                     autoAck: true,
                                     consumer: consumer);

                Console.ReadLine();
            }
        }
    }
}
