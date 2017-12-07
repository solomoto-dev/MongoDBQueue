using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoQueue.Core;
using MongoQueue.Core.AgentAbstractions;

namespace MongoQueue
{
    public class MessagingDependencyRegistrator : IMessagingDependencyRegistrator
    {
        public void RegisterDefault(IServiceCollection registrator)
        {
            registrator.AddScoped<IMessageStatusManager, MessageStatusManager>();
            registrator.AddScoped<IListeningAgent, ListeningAgent>();
            registrator.AddScoped<IUnprocessedMessagesAgent, UnprocessedMessagesAgent>();
            registrator.AddScoped<IDocumentMappingInitializer, DocumentMappingInitializer>();
            registrator.AddScoped<ISubscriptionAgent, SubscriptionAgent>();
            registrator.AddScoped<IPublishingAgent, PublishingAgent>();
            registrator.AddScoped<IDeadLettersAgent, DeadLettersAgent>();
            registrator.AddScoped<ICollectionCreator, CollectionCreator>();
            registrator.AddScoped<MongoAgent>();

            IdGenerator.SetGenerator(() => ObjectId.GenerateNewId().ToString());
            new CoreMessagingDependencyRegistrator().RegisterDefault(registrator);
        }
    }
}