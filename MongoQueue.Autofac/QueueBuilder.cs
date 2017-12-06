using System;
using System.Collections.Generic;
using Autofac;
using MongoQueue.Core;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Autofac
{
    public class QueueBuilder
    {
        private QueueConfigurator _configurator;
        private readonly Dictionary<Type, Type> _handlersRegistry = new Dictionary<Type, Type>();
        private ContainerBuilder _builder;

        public QueueBuilder AddAutofac<TRegistrator>(ContainerBuilder builder) where TRegistrator: IMessagingDependencyRegistrator, new()
        {
            var registrator = new AutofacRegistrator(builder);
            _builder = builder;
            _configurator = new QueueConfigurator(registrator, new TRegistrator());
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

        public void Build<TResolver>() where TResolver : IInstanceResolver
        {
            _builder.RegisterInstance(_configurator).As<QueueConfigurator>().SingleInstance();
            _builder.RegisterType<TResolver>()
                .As<IInstanceResolver>()
                .AsSelf()
                .SingleInstance();
            _builder.Register(ProviderFactory).As<QueueProvider>().SingleInstance();
        }

        private QueueProvider ProviderFactory(IComponentContext arg)
        {
            var resolver = arg.Resolve<IInstanceResolver>();
            return new QueueProvider(_configurator, resolver, _handlersRegistry);
        }        
    }
}