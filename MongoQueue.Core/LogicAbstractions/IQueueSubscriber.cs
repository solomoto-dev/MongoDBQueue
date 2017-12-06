using System;

namespace MongoQueue.Core.LogicAbstractions
{
    public interface IQueueSubscriber
    {
        void Subscribe<THandler, TMessage>() where THandler : IHandler<TMessage>;
        void Subscribe(Type handler, Type message);
    }
}