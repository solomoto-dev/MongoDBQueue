using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using MongoQueue;
using MongoQueue.Core;
using MongoQueue.Core.IntegrationDefaults;

namespace MongoQueueReaderNoAutofac
{
    class Program
    {
        static void Main(string[] args)
        {
            string route = "listener1";
            if (args.Any())
            {
                route = args[0];
            }

            var services = new ServiceCollection();
            var messagingConfiguration = DefaultMessagingConfiguration.Create();

            new QueueBuilder()
                .AddRegistrator<MessagingDependencyRegistrator>(services)
                .AddConfiguration(messagingConfiguration)
                .AddHandler<DefaultHandler, DomainMessage>()
                .AddResolver()
                .Build();

            var provider = services.BuildServiceProvider();

            var queue = provider.GetService<QueueProvider>();
            queue.Listen(route, CancellationToken.None).Wait();
            Console.WriteLine($"started listener {route}");
            Console.ReadLine();
        }
    }
}
