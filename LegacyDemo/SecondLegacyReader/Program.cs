using System;
using System.Linq;
using System.Threading;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MongoQueue.Autofac;
using MongoQueue.Core;
using MongoQueue.Legacy;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SecondLegacyReader
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
            var autofacRegistrator = new AutofacRegistrator(containerBuilder);

            var serviceProvider = new ServiceCollection
            {
                new ServiceDescriptor(
                    typeof(IContainer),
                    provider => autofacRegistrator.Container,
                    ServiceLifetime.Singleton)
            };

            containerBuilder.Populate(serviceProvider);
            var configurator = new QueueConfigurator(autofacRegistrator, new LegacyMessagingDependencyRegistrator())
                .RegisterHandler<DefaultHandler>()
                .RegisterHandler<AnotherDefaultHandler>();
            var builder = configurator.Build(autofacRegistrator.CreateResolver());
            var subscriber = builder.GetSubscriber();
            subscriber.Subscribe<DefaultHandler, DomainMessage>();
            subscriber.Subscribe<AnotherDefaultHandler, AnotherDomainMessage>();

            var mongoMessageListener = builder.GetListener();
            mongoMessageListener.Start(route, CancellationToken.None).Wait();
            Console.WriteLine($"started listener {route}");
            Console.ReadLine();
        }
    }
}