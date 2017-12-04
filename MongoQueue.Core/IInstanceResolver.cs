using System;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace MongoQueue.Core
{
    public interface IInstanceResolver
    {
        T Get<T>();
        object Get(Type t);
        IServiceScope CreateLifeTimeScope();
    }
}