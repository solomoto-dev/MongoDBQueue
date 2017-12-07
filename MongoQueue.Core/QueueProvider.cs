using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core
{
    public class QueueProvider
    {
        private readonly QueueConfigurator _configurator;
        private readonly IInstanceResolver _resolver;
        private readonly Dictionary<Type, Type> _handlersRegistry;
        private ConfiguredQueueBuilder _builder;

        public QueueProvider(QueueConfigurator configurator, IInstanceResolver resolver, Dictionary<Type, Type> handlersRegistry)
        {
            _configurator = configurator;
            _resolver = resolver;
            _handlersRegistry = handlersRegistry;
        }

        public Task Listen(string route, CancellationToken token)
        {
            var queueBuilder = _builder ?? (_builder = _configurator.Build(_resolver));
            var subscriber = queueBuilder.GetSubscriber();
            foreach (var pair in _handlersRegistry)
            {
                subscriber.Subscribe(pair.Key, pair.Value);
            }
            var listener = queueBuilder.GetListener();
            return listener.Start(route, token);
        }

        public IQueuePublisher GetPublisher()
        {
            var queueBuilder = _builder ?? (_builder = _configurator.Build(_resolver));
            return queueBuilder.GetPublisher();
        }
    }
}