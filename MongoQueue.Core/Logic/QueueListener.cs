using System;
using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core.AgentAbstractions;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core.Logic
{
    public class QueueListener
    {
        private readonly IMessageTypesCache _messageTypesCache;
        private readonly IMessagingLogger _messagingLogger;
        private readonly UnprocessedMessagesResender _unprocessedMessagesResender;
        private readonly IMessagingConfiguration _messagingConfiguration;
        private readonly IListeningAgent _listeningAgent;
        private readonly IDocumentMappingInitializer _documentMappingInitializer;
        private readonly ISubscriptionAgent _subscriptionAgent;
        private readonly ICollectionCreator _collectionCreator;

        public QueueListener(
            IMessageTypesCache messageTypesCache,
            IMessagingLogger messagingLogger,
            UnprocessedMessagesResender unprocessedMessagesResender,
            IMessagingConfiguration messagingConfiguration,
            IListeningAgent listeningAgent,
            IDocumentMappingInitializer documentMappingInitializer,
            ISubscriptionAgent subscriptionAgent,
            ICollectionCreator collectionCreator
        )
        {
            _messageTypesCache = messageTypesCache;
            _messagingLogger = messagingLogger;
            _unprocessedMessagesResender = unprocessedMessagesResender;
            _messagingConfiguration = messagingConfiguration;
            _listeningAgent = listeningAgent;
            _documentMappingInitializer = documentMappingInitializer;
            _subscriptionAgent = subscriptionAgent;
            _collectionCreator = collectionCreator;
        }

        public async Task Start(string appName, CancellationToken cancellationToken)
        {
            _documentMappingInitializer.Initialize();
            if (appName.Contains(" "))
            {
                throw new ArgumentException("appName");
            }
            var topics = _messageTypesCache.GetAllTopics();
            await _subscriptionAgent.UpdateSubscriber(appName, topics);
            await _collectionCreator.CreateCollectionIfNotExist(appName);
            RunListener(appName, cancellationToken);
        }

        private async void RunListener(string appName, CancellationToken cancellationToken)
        {
            try
            {
                _unprocessedMessagesResender.Start(appName, _messagingConfiguration.ResendInterval, cancellationToken);
                while (true)
                {
                    try
                    {
                        await _listeningAgent.Listen(appName, cancellationToken);
                    }
                    catch (TaskCanceledException)
                    {
                        _messagingLogger.Info($"{appName} cancelled listener");
                        return;
                    }
                    
                    catch (Exception e)
                    {
                        _messagingLogger.Error(e, $"error on listening in {appName}, trying againg");
                    }
                    await Task.Delay(500, cancellationToken);
                }
            }
            catch (Exception e)
            {
                _messagingLogger.Error(e, $"error on starting listening in {appName}, listener is stopped");
            }
        }
    }
}