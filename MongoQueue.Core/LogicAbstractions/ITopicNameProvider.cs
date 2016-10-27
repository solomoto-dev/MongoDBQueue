namespace MongoQueue.Core.LogicAbstractions
{
    public interface ITopicNameProvider
    {
        string Get<TMessage>();
    }
}