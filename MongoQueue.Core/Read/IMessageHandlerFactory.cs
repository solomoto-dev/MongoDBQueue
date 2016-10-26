using System;

namespace MongoQueue.Core.Read
{
    public interface IMessageHandlerFactory
    {
        IHandler Create(Type handlerType);
    }
}