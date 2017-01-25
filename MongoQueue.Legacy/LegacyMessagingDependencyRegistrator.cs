using MongoDB.Bson;
using MongoQueue.Core;
using MongoQueue.Core.AgentAbstractions;

namespace MongoQueue.Legacy
{
    public class LegacyMessagingDependencyRegistrator : IMessagingDependencyRegistrator
    {
        public void RegisterDefault(IRegistrator registrator)
        {
            registrator.Register<IMessageStatusManager, LegacyMessageStatusManager>();
            registrator.Register<IListeningAgent, LegacyListeningAgent>();
            registrator.Register<IUnprocessedMessagesAgent, LegacyUnprocessedMessagesAgent>();
            registrator.Register<IDocumentMappingInitializer, DocumentMappingInitializer>();
            registrator.Register<ISubscriptionAgent, LegacySubscriptionAgent>();
            registrator.Register<IPublishingAgent, LegacyPublishingAgent>();
            registrator.Register<IDeadLettersAgent, LegacyDeadLettersAgent>();
            registrator.Register<ICollectionCreator, LegacyCollectionCreator>();
            registrator.Register<LegacyMongoAgent>();
            IdGenerator.SetGenerator(() => ObjectId.GenerateNewId().ToString());
            new CoreMessagingDependencyRegistrator().RegisterDefault(registrator);
        }
    }
}