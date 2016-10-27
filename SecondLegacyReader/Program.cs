using System;
using System.Linq;
using System.Threading;
using Autofac;
using MongoQueue.Autofac;
using MongoQueue.Core.Logic;
using MongoQueue.Core.LogicAbstractions;
using MongoQueue.Legacy;

namespace SecondLegacyReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string appName = "listener2";
            if (args.Any())
            {
                appName = args[0];
            }

            AutofacComposition.Compose(new LegacyMessagingDependencyRegistrator(), b =>
            {
                b.RegisterType<DefaultHandler>();
                b.RegisterType<AnotherDefaultHandler>();
            });

            var subscriber = AutofacComposition.Container.Resolve<IQueueSubscriber>();
            subscriber.Subscribe<DefaultHandler, DomainMessage>();
            subscriber.Subscribe<AnotherDefaultHandler, AnotherDomainMessage>();

            var mongoMessageListener = AutofacComposition.Container.Resolve<MessageListener>();
            mongoMessageListener.Start(appName, CancellationToken.None).Wait();
            Console.WriteLine($"started listener {appName}");
            Console.ReadLine();
        }
    }
}