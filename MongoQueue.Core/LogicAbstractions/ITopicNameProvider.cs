using System;

namespace MongoQueue.Core.LogicAbstractions
{
    public interface ITopicNameProvider
    {
        string Get<TMessage>();
        string Get(Type message);
    }
}