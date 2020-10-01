using System;
using System.Collections.Concurrent;
using System.Linq;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core.Logic
{
    public class MessageTypesCache : IMessageTypesCache
    {
        private static readonly ConcurrentDictionary<string, Type> Cache = new ConcurrentDictionary<string, Type>();
        private readonly ITopicNameProvider _topicNameProvider;

        public MessageTypesCache(ITopicNameProvider topicNameProvider)
        {
            _topicNameProvider = topicNameProvider;
        }

        public void Register<TMessage>()
        {
            Register(typeof(TMessage));
        }

        public void Register(Type message)
        {
            var topic = _topicNameProvider.Get(message);
            Cache[topic] = message;
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