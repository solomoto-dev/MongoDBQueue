namespace MongoQueue.Core.LogicAbstractions
{
    public interface IQueueSubscriber
    {
        void Subscribe<THandler, TMessage>() where THandler : IHandler<TMessage>;
    }
}