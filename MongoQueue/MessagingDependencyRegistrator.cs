using MongoDB.Bson;
using MongoQueue.Core;
using MongoQueue.Core.AgentAbstractions;

namespace MongoQueue
{
    public class MessagingDependencyRegistrator : IMessagingDependencyRegistrator
    {
        public void RegisterDefault(IRegistrator registrator)
        {
            registrator.Register<IMessageStatusManager, MessageStatusManager>();
            registrator.Register<IListeningAgent, ListeningAgent>();
            registrator.Register<IUnprocessedMessagesAgent, UnprocessedMessagesAgent>();
            registrator.Register<IDocumentMappingInitializer, DocumentMappingInitializer>();
            registrator.Register<ISubscriptionAgent, SubscriptionAgent>();
            registrator.Register<IPublishingAgent, PublishingAgent>();
            registrator.Register<ICollectionCreator, CollectionCreator>();

            registrator.Register<MongoAgent>();

            IdGenerator.SetGenerator(() => ObjectId.GenerateNewId().ToString());
            new CoreMessagingDependencyRegistrator().RegisterDefault(registrator);
        }
    }
}