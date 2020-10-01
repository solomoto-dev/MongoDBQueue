using Microsoft.Extensions.DependencyInjection;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.IntegrationDefaults;
using MongoQueue.Core.Logic;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core
{
    public class CoreMessagingDependencyRegistrator : IMessagingDependencyRegistrator
    {
        public void RegisterDefault(IServiceCollection registrator)
        {
            registrator.AddTransient<ITopicNameProvider, DefaultTopicNameProvider>();
            registrator.AddScoped<IQueueSubscriber, DefaultQueueSubscriber>();
            registrator.AddScoped<IQueuePublisher, QueuePublisher>();
            registrator.AddScoped<IMessageHandlerFactory, MessageHandlerFactory>();
            registrator.AddScoped<IMessagingConfiguration, DefaultMessagingConfiguration>();
            registrator.AddScoped<IMessagingLogger, ConsoleMessagingLogger>();

            registrator.AddScoped<QueueListener>();
            registrator.AddScoped<MessageProcessor>();
            registrator.AddScoped<UnprocessedMessagesResender>();

            registrator.AddSingleton<IMessageTypesCache, MessageTypesCache>();
            registrator.AddSingleton<IMessageHandlersCache, MessageHandlersCache>();

            registrator.AddScoped<IMessagingConfiguration>(_=>DefaultMessagingConfiguration.Create());
        }
    }
}