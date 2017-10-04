using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core
{
    public class QueueConfigurator
    {
        private readonly IRegistrator _registrator;
        public QueueConfigurator(IRegistrator registrator, IMessagingDependencyRegistrator messagingDependencyRegistrator)
        {
            _registrator = registrator;
            messagingDependencyRegistrator.RegisterDefault(_registrator);
        }

        public QueueConfigurator SetConfigurationProvider<TConfiguration>(TConfiguration configuration = default(TConfiguration))
            where TConfiguration : class, IMessagingConfiguration
        {
            if (configuration == null)
            {
                _registrator.Register<IMessagingConfiguration, TConfiguration>();
            }
            else
            {
                _registrator.RegisterInstance<IMessagingConfiguration>(configuration);
            }
            return this;
        }

        public QueueConfigurator SetFactory<TFactory>(TFactory instance = null) where TFactory : class, IMessageHandlerFactory
        {
            if (instance == null)
            {
                _registrator.Register<IMessageHandlerFactory, TFactory>();
            }
            else
            {
                _registrator.RegisterInstance<IMessageHandlerFactory>(instance);
            }

            return this;
        }

        public QueueConfigurator SetTopicProvider<TTopicProvider>(TTopicProvider instance = null)
            where TTopicProvider : class, ITopicNameProvider
        {
            if (instance == null)
            {
                _registrator.Register<ITopicNameProvider, TTopicProvider>();
            }
            else
            {
                _registrator.RegisterInstance<ITopicNameProvider>(instance);
            }
            return this;
        }

        public QueueConfigurator SetLogger<TLogger>(TLogger instance = null)
            where TLogger : class, IMessagingLogger
        {
            if (instance == null)
            {
                _registrator.Register<IMessagingLogger, TLogger>();
            }
            else
            {
                _registrator.RegisterInstance<IMessagingLogger>(instance);
            }
            return this;
        }

        public QueueConfigurator RegisterHandler<THandler>() where THandler : class, IHandler
        {
            _registrator.Register<THandler>();
            return this;
        }

        public ConfiguredQueueBuilder Build(IInstanceResolver instanceResolver)
        {
            _registrator.RegisterInstance<IInstanceResolver>(instanceResolver);
            return new ConfiguredQueueBuilder(instanceResolver);
        }
    }
}