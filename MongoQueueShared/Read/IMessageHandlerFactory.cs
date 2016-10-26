using System;

namespace MongoQueueShared.Read
{
    public interface IMessageHandlerFactory
    {
        IHandler Create(Type handlerType);
    }
}