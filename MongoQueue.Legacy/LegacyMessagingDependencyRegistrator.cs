using System;
using MongoDB.Bson;
using MongoQueue.Core;
using MongoQueue.Core.AgentAbstractions;

namespace MongoQueue.Legacy
{
    public class LegacyMessagingDependencyRegistrator : IMessagingDependencyRegistrator
    {
        public void RegisterDefault(Action<Type, Type, bool> registerAbstract, Action<Type> registerClass)
        {
            registerAbstract(typeof(IMessageStatusManager), typeof(LegacyMessageStatusManager), false);
            registerAbstract(typeof(IListeningAgent), typeof(LegacyListeningAgent), false);
            registerAbstract(typeof(IUnprocessedMessagesAgent), typeof(LegacyUnprocessedMessagesAgent), false);
            registerAbstract(typeof(IDocumentMappingInitializer), typeof(DocumentMappingInitializer), false);
            registerAbstract(typeof(ISubscriptionAgent), typeof(LegacySubscriptionAgent), false);
            registerAbstract(typeof(IPublishingAgent), typeof(LegacyPublishingAgent), false);
            registerAbstract(typeof(ICollectionCreator), typeof(LegacyCollectionCreator), false);

            registerClass(typeof(LegacyMongoAgent));
            
            IdGenerator.SetGenerator(() => ObjectId.GenerateNewId().ToString());
            new CoreMessagingDependencyRegistrator().RegisterDefault(registerAbstract, registerClass);
        }
    }
}