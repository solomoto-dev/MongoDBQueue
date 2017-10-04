namespace MongoQueue.Core
{
    public interface IMessagingDependencyRegistrator
    {
        void RegisterDefault(IRegistrator registrator);
    }
}