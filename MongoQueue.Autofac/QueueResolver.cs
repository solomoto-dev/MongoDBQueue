using System;
using Microsoft.Extensions.DependencyInjection;
using MongoQueue.Core;

namespace MongoQueue.Autofac
{
    public class QueueResolver : IInstanceResolver
    {
        private readonly IServiceProvider _provider;

        public QueueResolver(IServiceProvider provider)
        {
            _provider = provider;
        }

        public T Get<T>()
        {
            return _provider.GetService<T>();
        }

        public object Get(Type t)
        {
            return _provider.GetService(t);
        }

        public IServiceScope CreateLifeTimeScope()
        {
            return _provider.CreateScope();
        }
    }
}