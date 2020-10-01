using System;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core.Logic
{
    public class DefaultQueueSubscriber : IQueueSubscriber
    {
        private readonly IMessageHandlersCache _messageHandlersCache;
        private readonly IMessageTypesCache _messageTypesCache;

        public DefaultQueueSubscriber(IMessageHandlersCache messageHandlersCache, IMessageTypesCache messageTypesCache)
        {
            _messageHandlersCache = messageHandlersCache;
            _messageTypesCache = messageTypesCache;
        }

        public void Subscribe<THandler, TMessage>() where THandler : IHandler<TMessage>
        {
            _messageTypesCache.Register<TMessage>();
            _messageHandlersCache.Register<THandler, TMessage>();
        }

        public void Subscribe(Type handler, Type message)
        {
            _messageTypesCache.Register(message);
            _messageHandlersCache.Register(handler, message);
        }
    }
}