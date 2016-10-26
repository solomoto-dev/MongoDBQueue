using System;
using System.Linq;
using System.Threading;
using MongoQueueShared.Common;
using MongoQueueShared.Read;

namespace MongoQueueReader
{
    class Program
    {
        static void Main(string[] args)
        {
            string appName = "listener";
            if (args.Any())
            {
                appName = args[0];
            }
            var messagingConfiguration = new DefaultMessagingConfiguration(null, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30));
            var topicNameProvider = new TopicNameProvider();
            var mongoHelper = new MongoMessagingAgent(messagingConfiguration);
            var messageTypesCache = new MessageTypesCache(topicNameProvider);
            var messageHandlersCache = new MessageHandlersCache(topicNameProvider);



            messageTypesCache.Register<DomainMessage>();
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