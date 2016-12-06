using MongoQueue.Core.Logic;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core
{
    public class ConfiguredQueueBuilder
    {
        private readonly IInstanceResolver _instanceResolver;

        public ConfiguredQueueBuilder(IInstanceResolver instanceResolver)
        {
            _instanceResolver = instanceResolver;
        }
        public QueueListener GetListener()
        {
            return _instanceResolver.Get<QueueListener>();
        }

        public IQueuePublisher GetPublisher()
        {
            return _instanceResolver.Get<IQueuePublisher>();
        }

        public IQueueSubscriber GetSubscriber()
        {
            return _instanceResolver.Get<IQueueSubscriber>();
        }
    }
}