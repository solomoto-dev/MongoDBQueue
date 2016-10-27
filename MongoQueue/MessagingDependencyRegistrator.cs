using System;
using MongoDB.Bson;
using MongoQueue.Core;
using MongoQueue.Core.AgentAbstractions;

namespace MongoQueue
{
    public class MessagingDependencyRegistrator : IMessagingDependencyRegistrator
    {
        public void RegisterDefault(Action<Type, Type, bool> registerAbstract, Action<Type> registerClass)
        {
            registerAbstract(typeof(IMessageStatusManager), typeof(MessageStatusManager), false);
            registerAbstract(typeof(IListeningAgent), typeof(ListeningAgent), false);
            registerAbstract(typeof(IUnprocessedMessagesAgent), typeof(UnprocessedMessagesAgent), false);
            registerAbstract(typeof(IDocumentMappingInitializer), typeof(DocumentMappingInitializer), false);
            registerAbstract(typeof(ISubscriptionAgent), typeof(SubscriptionAgent), false);
            registerAbstract(typeof(IPublishingAgent), typeof(PublishingAgent), false);
            registerAbstract(typeof(ICollectionCreator), typeof(CollectionCreator), false);

            registerClass(typeof(MongoAgent));
            
            IdGenerator.SetGenerator(() => ObjectId.GenerateNewId().ToString());

            new CoreMessagingDependencyRegistrator().RegisterDefault(registerAbstract, registerClass);
        }
    }
}