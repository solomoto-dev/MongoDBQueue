using System;

namespace MongoQueue.Core.Read
{
    public interface IMessageHandlersCache
    {
        MessageHandlersCache Register<THandler, TMessage>() where THandler : IHandler<TMessage>;
        Type Get(string topic);
    }
}