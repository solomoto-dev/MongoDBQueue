using System;
using Microsoft.Extensions.DependencyInjection;

namespace MongoQueue.Core
{
    public class ServiceProviderResolver : IInstanceResolver
    {
        private readonly IServiceProvider _provider;

        public ServiceProviderResolver(IServiceProvider provider)
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