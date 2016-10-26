using System;

namespace MongoQueueShared.Read
{
    public interface IMessageTypesCache
    {
        void Register<TMessage>();
        Type Get(string topic);
        string[] GetAllTopics();
    }
}