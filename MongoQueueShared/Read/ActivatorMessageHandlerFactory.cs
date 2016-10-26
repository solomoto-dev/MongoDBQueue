using System;

namespace MongoQueueShared.Read
{
    public class ActivatorMessageHandlerFactory : IMessageHandlerFactory
    {
        public IHandler Create(Type handlerType)
        {
            return (IHandler) Activator.CreateInstance(handlerType);
        }
    }
}