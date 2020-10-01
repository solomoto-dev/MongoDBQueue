using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core
{
    public class QueueBuilder
    {
        private QueueConfigurator _configurator;
        private readonly Dictionary<Type, Type> _handlersRegistry = new Dictionary<Type, Type>();
        public IServiceCollection Registrator { get; private set; } = new ServiceCollection();

        public QueueBuilder AddRegistrator<TRegistrator>(IServiceCollection registrator = null) where TRegistrator : IMessagingDependencyRegistrator, new()
        {
            if(registrator != null)
                Registrator = registrator;
            _configurator = new QueueConfigurator(Registrator, new TRegistrator());
            return this;
        }

        public QueueBuilder AddLogger<TLogger>(TLogger logger=null) where TLogger : class, IMessagingLogger
        {
            _configurator.SetLogger(logger);
            return this;
        }

        public QueueBuilder AddConfiguration<TConfiguration>(TConfiguration configuration = null)
            where TConfiguration : class, IMessagingConfiguration
        {
            _configurator.SetConfigurationProvider(configuration);
            return this;
        }

        public QueueBuilder AddHandler<THandler, TMessage>() where THandler : class, IHandler<TMessage>
        {
            _configurator.RegisterHandler<THandler>();
            _handlersRegistry.Add(typeof(THandler), typeof(TMessage));
            return this;
        }

        public QueueBuilder AddResolver()
        {
            Registrator.AddSingleton(_configurator);
            Registrator.AddSingleton<IInstanceResolver, ServiceProviderResolver>();
            Registrator.AddSingleton(ProviderFactory);
            return this;
        }

        public void Build()
        {            
        }

        private QueueProvider ProviderFactory(IServiceProvider arg)
        {
            var resolver = arg.GetService<IInstanceResolver>();
            return new QueueProvider(_configurator, resolver, _handlersRegistry);
        }
    }
}