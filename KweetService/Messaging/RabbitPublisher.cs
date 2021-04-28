using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KweetService.Models;
using RabbitMQ.Client;

namespace KweetService.Messaging
{
    public static class RabbitPublisher
    {
        public static void PublishMessage(Kweet kweet)
        {
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

                string jsonMessage = JsonSerializer.Serialize(kweet);
                byte[] body = Encoding.UTF8.GetBytes(jsonMessage);

                channel.BasicPublish(exchange: "",
                                     routingKey: "kweets",
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine(" [x] Sent {0}", jsonMessage);
            }
        }
    }
}
