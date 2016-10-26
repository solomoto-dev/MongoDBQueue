using System;
using System.Linq;
using System.Threading;
using MongoQueueShared.Common;
using MongoQueueShared.Read;

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
            var messagingConfiguration = new DefaultMessagingConfiguration(null, TimeSpan.FromMilliseconds(300), TimeSpan.FromSeconds(1));
            var topicNameProvider = new TopicNameProvider();
            var messageTypesCache = new MessageTypesCache(topicNameProvider);
            var messageHandlersCache = new MessageHandlersCache(topicNameProvider);
            var mongoHelper = new MongoMessagingAgent(messagingConfiguration);

            messageTypesCache.Register<DomainMessage>();
            messageTypesCache.Register<AnotherDomainMessage>();

            messageHandlersCache.Register<AnotherDefaultHandler, AnotherDomainMessage>();
            messageHandlersCache.Register<DefaultHandler, DomainMessage>();

            var consoleMessagingLogger = new ConsoleMessagingLogger();
            var messageProcessor = new MessageProcessor(messageHandlersCache, messageTypesCache, new ActivatorMessageHandlerFactory(), consoleMessagingLogger);
            var mongoMessageListener = new MongoMessageListener(messageTypesCache, mongoHelper, messagingConfiguration, consoleMessagingLogger, messageProcessor);
            mongoMessageListener.Start(appName, CancellationToken.None).Wait();
            Console.WriteLine($"started listener {appName}");
            Console.ReadLine();
        }
    }
}
