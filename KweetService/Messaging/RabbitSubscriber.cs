using KweetService.DAL.Contexts;
using KwetterShared.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace KweetService.Messaging
{
    public class RabbitSubscriber : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public RabbitSubscriber(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void OpenChannel()
        {
            Console.WriteLine("Opening channel...");
            string host = Environment.GetEnvironmentVariable("RabbitHost");
            var factory = new ConnectionFactory() { HostName = host };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "deletion", type: ExchangeType.Fanout);

                string queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                  exchange: "deletion",
                                  routingKey: "");

                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    byte[] body = ea.Body.ToArray();
                    string message = Encoding.UTF8.GetString(body);

                    //string username = JsonSerializer.Deserialize<string>(message);
                    string username = message;

                    await DeleteUsernameFromKweets(username);

                    Console.WriteLine(" [x] Received {0}", message);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                Console.ReadLine();
            }
        }

        private async Task DeleteUsernameFromKweets(string username)
        {
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                KweetDBContext context = scope.ServiceProvider.GetRequiredService<KweetDBContext>();

                
                IQueryable<Kweet> kweets = context.Kweets.Where(kweet => kweet.Username == username);
                foreach(Kweet kweet in kweets)
                {
                    kweet.Username = "[Deleted]";
                }
                context.UpdateRange(kweets);                
                await context.SaveChangesAsync();
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            OpenChannel();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Closing?");
            return Task.CompletedTask;
        }
    }
}
