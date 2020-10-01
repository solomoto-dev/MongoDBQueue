using System;
using System.Linq;
using System.Threading;
using Autofac;
using MongoQueue;
using MongoQueue.Autofac;
using MongoQueue.Core;

namespace SecondReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string route = "listener2";
            if (args.Any())
            {
                route = args[0];
            }

            var containerBuilder = new ContainerBuilder();

            new QueueBuilder()
                .AddRegistrator<MessagingDependencyRegistrator>()
                .AddHandler<DefaultHandler, DomainMessage>()
                .AddHandler<AnotherDefaultHandler, AnotherDomainMessage>()
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