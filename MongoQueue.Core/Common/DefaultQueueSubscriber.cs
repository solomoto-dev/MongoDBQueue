using MongoQueue.Core.Read;

namespace MongoQueue.Core.Common
{
    public class DefaultQueueSubscriber : IQueueSubscriber
    {
        private readonly MessageHandlersCache _messageHandlersCache;
        private readonly MessageTypesCache _messageTypesCache;

        public DefaultQueueSubscriber(MessageHandlersCache messageHandlersCache, MessageTypesCache messageTypesCache)
        {
            _messageHandlersCache = messageHandlersCache;
            _messageTypesCache = messageTypesCache;
        }

        public void Subscribe<THandler, TMessage>() where THandler : IHandler<TMessage>
        {
            _messageTypesCache.Register<TMessage>();
            _messageHandlersCache.Register<THandler, TMessage>();
        }
    }
}