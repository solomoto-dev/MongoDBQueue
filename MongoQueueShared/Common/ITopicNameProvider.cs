namespace MongoQueueShared.Common
{
    public interface ITopicNameProvider
    {
        string Get<TMessage>();
    }
}