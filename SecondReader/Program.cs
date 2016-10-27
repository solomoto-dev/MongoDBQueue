using System;
using System.Linq;
using System.Threading;
using Autofac;
using MongoQueue;
using MongoQueue.Autofac;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.IntegrationDefaults;
using MongoQueue.Core.Logic;
using MongoQueue.Core.LogicAbstractions;

namespace SecondReader
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

            AutofacComposition.Compose(new MessagingDependencyRegistrator(), b =>
            {
                b.RegisterInstance(new DefaultMessagingConfiguration("mongodb://localhost:27017/dev-queue", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30))).As<IMessagingConfiguration>();
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