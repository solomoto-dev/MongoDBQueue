using System.Runtime.InteropServices;
using Autofac;
using MongoQueue.Core;

namespace MongoQueue.Autofac
{
    public class AutofacRegistrator : IRegistrator
    {
        private readonly ContainerBuilder _container;

        public AutofacRegistrator()
        {
            _container = new ContainerBuilder();
        }

        public void Register<TAbst, TImpl>() where TImpl : class, TAbst
        {
            _container.RegisterType<TImpl>().As<TAbst>();
        }

        public void Register<TImpl>() where TImpl : class
        {
            _container.RegisterType<TImpl>();
        }

        public void RegisterSingleton<TAbst, TImpl>() where TImpl : class, TAbst
        {
            _container.RegisterType<TImpl>().As<TAbst>().SingleInstance();
        }

        public void RegisterInstance<T>(T instance) where T : class
        {
            _container.RegisterInstance(instance).As<T>();
        }

        public IInstanceResolver CreateResolver()
        {
            var instanceResolver = new AutofacResolver();
            _container.RegisterInstance(instanceResolver).As<IInstanceResolver>();
            var container = _container.Build();
            instanceResolver.Initialize(container);
            return instanceResolver;
        }
    }
}