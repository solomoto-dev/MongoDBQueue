using System;
using Autofac;
using MongoQueue.Core;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.IntegrationDefaults;

namespace MongoQueue.Autofac
{
    public class AutofacComposition
    {
        public static IContainer Container { get; private set; }
        public static void Compose(IMessagingDependencyRegistrator registrator, Action<ContainerBuilder> overrides = null)
        {
            var builder = new ContainerBuilder();

            Action<Type, Type, bool> registerAbstract = (abstaction, implementation, isSingleton) =>
            {
                if (isSingleton)
                {
                    builder.RegisterType(implementation).As(abstaction).SingleInstance();
                }
                else
                {
                    builder.RegisterType(implementation).As(abstaction).InstancePerDependency();
                }
            };
            Action<Type> registerClass = type => builder.RegisterType(type).AsSelf();
            registrator.RegisterDefault(registerAbstract, registerClass);

            builder.RegisterType<AutofacHandlerFactory>().As<IMessageHandlerFactory>();

            if (overrides != null)
            {
                overrides(builder);
            }
            Container = builder.Build();
        }
    }
}
