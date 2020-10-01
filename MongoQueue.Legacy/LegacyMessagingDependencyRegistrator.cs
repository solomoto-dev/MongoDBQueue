using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoQueue.Core;
using MongoQueue.Core.AgentAbstractions;

namespace MongoQueue.Legacy
{
    public class LegacyMessagingDependencyRegistrator : IMessagingDependencyRegistrator
    {
        public void RegisterDefault(IServiceCollection registrator)
        {
            registrator.AddScoped<IMessageStatusManager, LegacyMessageStatusManager>();
            registrator.AddScoped<IListeningAgent, LegacyListeningAgent>();
            registrator.AddScoped<IUnprocessedMessagesAgent, LegacyUnprocessedMessagesAgent>();
            registrator.AddScoped<IDocumentMappingInitializer, DocumentMappingInitializer>();
            registrator.AddScoped<ISubscriptionAgent, LegacySubscriptionAgent>();
            registrator.AddScoped<IPublishingAgent, LegacyPublishingAgent>();
            registrator.AddScoped<IDeadLettersAgent, LegacyDeadLettersAgent>();
            registrator.AddScoped<ICollectionCreator, LegacyCollectionCreator>();
            registrator.AddScoped<LegacyMongoAgent>();
            IdGenerator.SetGenerator(() => ObjectId.GenerateNewId().ToString());
            new CoreMessagingDependencyRegistrator().RegisterDefault(registrator);
        }
    }
}