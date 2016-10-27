using System;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core.IntegrationDefaults
{
    public class ActivatorMessageHandlerFactory : IMessageHandlerFactory
    {
        public IHandler Create(Type handlerType)
        {
            return (IHandler) Activator.CreateInstance(handlerType);
        }
    }
}