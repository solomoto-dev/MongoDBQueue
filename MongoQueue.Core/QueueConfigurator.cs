using Microsoft.Extensions.DependencyInjection;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core
{
    public class QueueConfigurator
    {
        private readonly IServiceCollection _registrator;
        public QueueConfigurator(IServiceCollection registrator, IMessagingDependencyRegistrator messagingDependencyRegistrator)
        {
            _registrator = registrator;
            messagingDependencyRegistrator.RegisterDefault(_registrator);
        }

        public QueueConfigurator SetConfigurationProvider<TConfiguration>(TConfiguration configuration = default(TConfiguration))
            where TConfiguration : class, IMessagingConfiguration
        {
            if (configuration == null)
            {
                _registrator.AddScoped<IMessagingConfiguration, TConfiguration>();
            }
            else
            {
                _registrator.AddScoped<IMessagingConfiguration>(_ => configuration);
            }
            return this;
        }

        public QueueConfigurator SetFactory<TFactory>(TFactory instance = null) where TFactory : class, IMessageHandlerFactory
        {
            if (instance == null)
            {
                _registrator.AddScoped<IMessageHandlerFactory, TFactory>();
            }
            else
            {
                _registrator.AddScoped<IMessageHandlerFactory>(_ => instance);
            }

            return this;
        }

        public QueueConfigurator SetTopicProvider<TTopicProvider>(TTopicProvider instance = null)
            where TTopicProvider : class, ITopicNameProvider
        {
            if (instance == null)
            {
                _registrator.AddScoped<ITopicNameProvider, TTopicProvider>();
            }
            else
            {
                _registrator.AddScoped<ITopicNameProvider>(_ => instance);
            }
            return this;
        }

        public QueueConfigurator SetLogger<TLogger>(TLogger instance = null)
            where TLogger : class, IMessagingLogger
        {
            if (instance == null)
            {
                _registrator.AddScoped<IMessagingLogger, TLogger>();
            }
            else
            {
                _registrator.AddScoped<IMessagingLogger>(_ => instance);
            }
            return this;
        }

        public QueueConfigurator RegisterHandler<THandler>() where THandler : class, IHandler
        {
            _registrator.AddScoped<THandler>();
            return this;
        }

        public ConfiguredQueueBuilder Build(IInstanceResolver instanceResolver)
        {
            _registrator.AddScoped(_ => instanceResolver);
            return new ConfiguredQueueBuilder(instanceResolver);
        }
    }
}