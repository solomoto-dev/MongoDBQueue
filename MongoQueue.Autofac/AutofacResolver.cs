using System;
using Autofac;
using MongoQueue.Core;

namespace MongoQueue.Autofac
{
    public class AutofacResolver : IInstanceResolver
    {
        private IContainer _container;

        internal void Initialize(IContainer container)
        {
            _container = container;
        }
        public T Get<T>()
        {
            return _container.Resolve<T>();
        }

        public object Get(Type t)
        {
            return _container.Resolve(t);
        }

        public ILifetimeScope CreateLifeTimeScope()
        {
            return _container.BeginLifetimeScope();
        }
    }
}