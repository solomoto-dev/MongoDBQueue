using System;
using System.Linq;
using System.Threading;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MongoQueue;
using MongoQueue.Autofac;
using MongoQueue.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
            IContainer container = null;

            var serviceProvider = new ServiceCollection
            {
                new ServiceDescriptor(
                    typeof(IContainer),
                    provider => container,
                    ServiceLifetime.Singleton)
            };

            containerBuilder.Populate(serviceProvider);
            new QueueBuilder()
                .AddAutofac<MessagingDependencyRegistrator>(containerBuilder)
                .AddHandler<DefaultHandler, DomainMessage>()
                .AddHandler<AnotherDefaultHandler, AnotherDomainMessage>()
                .Build<ServiceProviderResolver>();
            container = containerBuilder.Build();
            var queue = container.Resolve<QueueProvider>();
            queue.Listen(route, CancellationToken.None).Wait();
            
            Console.WriteLine($"started listener {route}");
            Console.ReadLine();
        }
    }
}