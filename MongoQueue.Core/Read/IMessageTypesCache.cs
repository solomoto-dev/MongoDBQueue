using System;

namespace MongoQueue.Core.Read
{
    public interface IMessageTypesCache
    {
        void Register<TMessage>();
        Type Get(string topic);
        string[] GetAllTopics();
    }
}