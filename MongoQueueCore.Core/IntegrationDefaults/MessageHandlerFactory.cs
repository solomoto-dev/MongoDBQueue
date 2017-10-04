using System;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core.IntegrationDefaults
{
    public class MessageHandlerFactory : IMessageHandlerFactory
    {
        private readonly IInstanceResolver _instanceResolver;

        public MessageHandlerFactory(IInstanceResolver instanceResolver)
        {
            _instanceResolver = instanceResolver;
        }
        public IHandler Create(Type handlerType)
        {
            return (IHandler)_instanceResolver.Get(handlerType);
        }
    }
}