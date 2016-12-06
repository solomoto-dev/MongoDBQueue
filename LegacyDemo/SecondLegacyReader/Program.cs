using System;
using System.Linq;
using System.Threading;
using MongoQueue.Autofac;
using MongoQueue.Core;
using MongoQueue.Legacy;

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
            var autofacRegistrator = new AutofacRegistrator();
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