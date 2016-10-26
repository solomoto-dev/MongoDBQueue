namespace MongoQueue.Core.Common
{
    public interface ITopicNameProvider
    {
        string Get<TMessage>();
    }
}