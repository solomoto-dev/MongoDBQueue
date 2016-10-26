namespace MongoQueueShared.Common
{
    public class TopicNameProvider : ITopicNameProvider
    {
        public virtual string Get<TMessage>()
        {
            return typeof(TMessage).Name;
        }
    }
}