using System;
using Autofac;

namespace MongoQueue.Core
{
    public interface IInstanceResolver
    {
        T Get<T>();
        object Get(Type t);
        ILifetimeScope CreateLifeTimeScope();
    }
}