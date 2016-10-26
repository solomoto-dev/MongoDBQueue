using System;
using System.Collections.Generic;
using System.Linq;
using MongoQueueShared.Common;

namespace MongoQueueShared.Read
{
    public class MessageTypesCache : IMessageTypesCache
    {
        private static readonly Dictionary<string, Type> Cache = new Dictionary<string, Type>();
        private readonly ITopicNameProvider _topicNameProvider;

        public MessageTypesCache(ITopicNameProvider topicNameProvider)
        {
            _topicNameProvider = topicNameProvider;
        }

        public void Register<TMessage>()
        {
            var topic = _topicNameProvider.Get<TMessage>();
            Cache[topic] = typeof(TMessage);
        }

        public Type Get(string topic)
        {
            return Cache[topic];
        }

        public string[] GetAllTopics()
        {
            return Cache.Keys.ToArray();
        }
    }
}