using MongoQueueShared.Read;

namespace MongoQueueShared.Common
{
    public interface IQueueSubscriber
    {
        void Subscribe<THandler, TMessage>() where THandler : IHandler<TMessage>;
    }
}