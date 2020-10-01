using System;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core.Logic
{
    public class DefaultTopicNameProvider : ITopicNameProvider
    {
        public virtual string Get<TMessage>()
        {
            return Get(typeof(TMessage));
        }

        public virtual string Get(Type message)
        {
            return message.Name;
        }
    }
}