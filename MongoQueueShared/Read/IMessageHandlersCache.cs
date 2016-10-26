using System;

namespace MongoQueueShared.Read
{
    public interface IMessageHandlersCache
    {
        MessageHandlersCache Register<THandler, TMessage>() where THandler : IHandler<TMessage>;
        Type Get(string topic);
    }
}