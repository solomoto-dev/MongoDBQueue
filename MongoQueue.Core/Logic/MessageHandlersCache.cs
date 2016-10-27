using System;
using System.Collections.Generic;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core.Logic
{
    public class MessageHandlersCache : IMessageHandlersCache
    {
        private readonly ITopicNameProvider _topicNameProvider;
        private static readonly Dictionary<string, Type> Cache = new Dictionary<string, Type>();

        public MessageHandlersCache(ITopicNameProvider topicNameProvider)
        {
            _topicNameProvider = topicNameProvider;
        }

        public MessageHandlersCache Register<THandler, TMessage>() where THandler : IHandler<TMessage>
        {
            var topic = _topicNameProvider.Get<TMessage>();
            Cache[topic] = typeof(THandler);
            return this;
        }

        public Type Get(string topic)
        {
            return Cache[topic];
        }
    }
}