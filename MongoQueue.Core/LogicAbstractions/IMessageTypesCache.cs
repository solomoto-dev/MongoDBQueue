using System;

namespace MongoQueue.Core.LogicAbstractions
{
    public interface IMessageTypesCache
    {
        void Register<TMessage>();
        Type Get(string topic);
        string[] GetAllTopics();
        void Register(Type message);
    }
}