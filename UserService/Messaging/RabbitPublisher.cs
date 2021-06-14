using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UserService.Messaging
{
    public class RabbitPublisher
    {
        public static void PublishMessage(string userID, string username)
        {
            string host = Environment.GetEnvironmentVariable("RabbitHost");
            var factory = new ConnectionFactory() { HostName = host };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "profiles",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var message = new { userID = userID, username = username };
                //string jsonMessage = userID;//JsonSerializer.Serialize(userID);
                string jsonMessage = JsonSerializer.Serialize(message);
                byte[] body = Encoding.UTF8.GetBytes(jsonMessage);

                channel.BasicPublish(exchange: "",
                                     routingKey: "profiles",
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine(" [x] Sent {0}", jsonMessage);
            }
        }

        // TODO: Make kweets have profileID by caching the ID in the JWT token, so users always make kweets with ID
        //       Then I can user UserID to remove everything in all services        
        public static void PublishMessageToExchange(string userID)
        {
            string host = Environment.GetEnvironmentVariable("RabbitHost");
            var factory = new ConnectionFactory() { HostName = host };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("deletion", ExchangeType.Fanout);

                string jsonMessage = userID;
                byte[] body = Encoding.UTF8.GetBytes(jsonMessage);

                channel.BasicPublish(exchange: "deletion",
                                     routingKey: "",
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}
