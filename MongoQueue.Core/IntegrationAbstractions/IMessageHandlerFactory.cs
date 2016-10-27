using System;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core.IntegrationAbstractions
{
    public interface IMessageHandlerFactory
    {
        IHandler Create(Type handlerType);
    }
}