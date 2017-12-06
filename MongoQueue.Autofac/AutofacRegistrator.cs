using Autofac;
using MongoQueue.Core;

namespace MongoQueue.Autofac
{
    public class AutofacRegistrator : IRegistrator
    {
        private readonly ContainerBuilder _builder;
        public IContainer Container { get; private set; }

        public AutofacRegistrator(ContainerBuilder builder)
        {
            _builder = builder;
        }

        public void Register<TAbst, TImpl>() where TImpl : class, TAbst
        {
            _builder.RegisterType<TImpl>().As<TAbst>();
        }

        public void Register<TImpl>() where TImpl : class
        {
            _builder.RegisterType<TImpl>().AsSelf();
        }

        public void RegisterSingleton<TAbst, TImpl>() where TImpl : class, TAbst
        {
            _builder.RegisterType<TImpl>().As<TAbst>().SingleInstance();
        }

        public void RegisterInstance<T>(T instance) where T : class
        {
            _builder.RegisterInstance(instance).As<T>();
        }

        public IInstanceResolver CreateResolver()
        {
            _builder.RegisterType<ServiceProviderResolver>()
                .As<IInstanceResolver>();
            Container = _builder.Build();
            return Container.Resolve<IInstanceResolver>();
        }
    }
}