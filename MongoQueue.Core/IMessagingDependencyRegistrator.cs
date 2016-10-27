using System;

namespace MongoQueue.Core
{
    public interface IMessagingDependencyRegistrator
    {
        void RegisterDefault(Action<Type, Type, bool> registerAbstract, Action<Type> registerClass);
    }
}