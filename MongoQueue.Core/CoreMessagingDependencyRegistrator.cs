using System;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.IntegrationDefaults;
using MongoQueue.Core.Logic;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core
{
    public class CoreMessagingDependencyRegistrator : IMessagingDependencyRegistrator
    {
        public void RegisterDefault(Action<Type, Type, bool> registerAbstract, Action<Type> registerClass)
        {
            registerAbstract(typeof(ITopicNameProvider), typeof(TopicNameProvider), false);
            registerAbstract(typeof(IQueueSubscriber), typeof(DefaultQueueSubscriber), false);
            registerAbstract(typeof(IQueuePublisher), typeof(QueuePublisher), false);
            registerAbstract(typeof(IMessageHandlerFactory), typeof(ActivatorMessageHandlerFactory), false);
            registerAbstract(typeof(IMessagingConfiguration), typeof(DefaultMessagingConfiguration), false);
            registerAbstract(typeof(IMessagingLogger), typeof(ConsoleMessagingLogger), false);

            registerClass(typeof(QueueListener));
            registerClass(typeof(MessageProcessor));
            registerClass(typeof(UnprocessedMessagesResender));

            registerAbstract(typeof(IMessageTypesCache), typeof(MessageTypesCache), true);
            registerAbstract(typeof(IMessageHandlersCache), typeof(MessageHandlersCache), true);
        }
    }
}