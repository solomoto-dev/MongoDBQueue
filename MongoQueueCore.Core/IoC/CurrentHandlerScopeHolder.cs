using Autofac;

namespace MongoQueue.Core.IoC
{
    public interface ICurrentHandlerScopeHolder
    {
        T Resolve<T>();
        void Init(ILifetimeScope lifetimeScope);
    }

    public class CurrentHandlerScopeHolder : ICurrentHandlerScopeHolder
    {
        private ILifetimeScope _lifetimeScope;

        public void Init(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public T Resolve<T>()
        {
            return _lifetimeScope.Resolve<T>();
        }
    }
}
