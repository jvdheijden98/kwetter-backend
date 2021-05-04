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
using TimelineService.DAL.Contexts;

namespace TimelineService.Messaging
{
    public class RabbitSubscriber : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
       // private readonly TimelineLogic _logic;

        public RabbitSubscriber(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            //using (IServiceScope scope = _scopeFactory.CreateScope())
            //{
            //    TimelineLogic logic = scope.ServiceProvider.GetRequiredService<TimelineLogic>();
            //    _logic = logic;
            //}
        }

        public Kweet LastKweet { get; private set; }
        public void OpenChannel()
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

                    CreateKweet();

                    Console.WriteLine(" [x] Received {0}", message);
                };
                channel.BasicConsume(queue: "kweets",
                                     autoAck: true,
                                     consumer: consumer);

                Console.ReadLine();
            }
        }

        private async Task CreateKweet()
        {
            //await _logic.CreateKweet(LastKweet);
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                TimelineDBContext context = scope.ServiceProvider.GetRequiredService<TimelineDBContext>();

                // TODO: In the future don't do this every tweet call, but rather every hour or so
                long oldDate = DateTimeOffset.Now.AddDays(-1).ToUnixTimeSeconds();
                IQueryable<Kweet> dates = context.Kweets.Where(kweet => kweet.TimeCreated < oldDate);
                context.Kweets.RemoveRange(dates);

                context.Add(LastKweet);
                context.SaveChanges();
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
