using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core.Logic
{
    public class TopicNameProvider : ITopicNameProvider
    {
        public virtual string Get<TMessage>()
        {
            return typeof(TMessage).Name;
        }
    }
}