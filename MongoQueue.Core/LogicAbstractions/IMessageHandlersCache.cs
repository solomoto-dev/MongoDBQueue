using System;
using MongoQueue.Core.Logic;

namespace MongoQueue.Core.LogicAbstractions
{
    public interface IMessageHandlersCache
    {
        MessageHandlersCache Register<THandler, TMessage>() where THandler : IHandler<TMessage>;
        Type Get(string topic);
        MessageHandlersCache Register(Type handler, Type message);
    }
}