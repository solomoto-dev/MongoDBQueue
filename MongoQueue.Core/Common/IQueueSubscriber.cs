using MongoQueue.Core.Read;

namespace MongoQueue.Core.Common
{
    public interface IQueueSubscriber
    {
        void Subscribe<THandler, TMessage>() where THandler : IHandler<TMessage>;
    }
}