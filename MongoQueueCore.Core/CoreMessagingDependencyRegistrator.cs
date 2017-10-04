using Autofac;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.IntegrationDefaults;
using MongoQueue.Core.IoC;
using MongoQueue.Core.Logic;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core
{
    public class CoreMessagingDependencyRegistrator : IMessagingDependencyRegistrator
    {
        public void RegisterDefault(IRegistrator registrator)
        {
            registrator.Register<ITopicNameProvider, DefaultTopicNameProvider>();
            registrator.Register<IQueueSubscriber, DefaultQueueSubscriber>();
            registrator.Register<IQueuePublisher, QueuePublisher>();
            registrator.Register<IMessageHandlerFactory, MessageHandlerFactory>();
            registrator.Register<IMessagingConfiguration, DefaultMessagingConfiguration>();
            registrator.Register<IMessagingLogger, ConsoleMessagingLogger>();
            registrator.Register<ICurrentHandlerScopeHolder, CurrentHandlerScopeHolder>();

            registrator.Register<QueueListener>();
            registrator.Register<MessageProcessor>();
            registrator.Register<UnprocessedMessagesResender>();

            registrator.RegisterSingleton<IMessageTypesCache, MessageTypesCache>();
            registrator.RegisterSingleton<IMessageHandlersCache, MessageHandlersCache>();

            registrator.RegisterInstance<IMessagingConfiguration>(DefaultMessagingConfiguration.Create());
        }
    }
}