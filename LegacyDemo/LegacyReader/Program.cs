using System;
using System.Linq;
using System.Threading;
using Autofac;
using MongoQueue.Autofac;
using MongoQueue.Legacy;
using MongoQueue.Core;

namespace LegacyReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string route = "listener";
            if (args.Any())
            {
                route = args[0];
            }

            var containerBuilder = new ContainerBuilder();

            new QueueBuilder()
                .AddRegistrator<LegacyMessagingDependencyRegistrator>()
                .AddHandler<DefaultHandler, DomainMessage>()
                .AddResolver()
                .AddAutofac(containerBuilder)
                .Build();

            var container = containerBuilder.Build();

            var queue = container.Resolve<QueueProvider>();
            queue.Listen(route, CancellationToken.None).Wait();

            Console.WriteLine($"started listener {route}");
            Console.ReadLine();
        }
    }
}