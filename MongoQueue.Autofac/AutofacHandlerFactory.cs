using System;
using Autofac;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Autofac
{
    public class AutofacHandlerFactory : IMessageHandlerFactory
    {
        public IHandler Create(Type handlerType)
        {
            return (IHandler)AutofacComposition.Container.Resolve(handlerType);
        }
    }
}