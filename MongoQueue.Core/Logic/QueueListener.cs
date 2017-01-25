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
        private readonly IPublishingAgent _publishingAgent;
        private readonly IDeadLettersAgent _deadLettersAgent;
        private readonly ICollectionCreator _collectionCreator;
        private bool _isStarted;

        public QueueListener(
            IMessageTypesCache messageTypesCache,
            IMessagingLogger messagingLogger,
            UnprocessedMessagesResender unprocessedMessagesResender,
            IMessagingConfiguration messagingConfiguration,
            IListeningAgent listeningAgent,
            IDocumentMappingInitializer documentMappingInitializer,
            ISubscriptionAgent subscriptionAgent,
            IPublishingAgent publishingAgent,
            ICollectionCreator collectionCreator, IDeadLettersAgent deadLettersAgent)
        {
            _messageTypesCache = messageTypesCache;
            _messagingLogger = messagingLogger;
            _unprocessedMessagesResender = unprocessedMessagesResender;
            _messagingConfiguration = messagingConfiguration;
            _listeningAgent = listeningAgent;
            _documentMappingInitializer = documentMappingInitializer;
            _subscriptionAgent = subscriptionAgent;
            _publishingAgent = publishingAgent;
            _collectionCreator = collectionCreator;
            _deadLettersAgent = deadLettersAgent;
        }

        public async Task Start(string route, CancellationToken cancellationToken)
        {
            //TODO::VZ:: refactor
            if (_isStarted)
            {
                throw new InvalidOperationException("listener is already running");
            }
            _isStarted = true;
            _documentMappingInitializer.Initialize();
            if (route.Contains(" "))
            {
                throw new ArgumentException("route");
            }
            var topics = _messageTypesCache.GetAllTopics();
            await _subscriptionAgent.UpdateSubscriber(route, topics);
            await _collectionCreator.CreateCollectionIfNotExist(route);
            RunListener(route, cancellationToken);
            ProcessDeadLetters(route, cancellationToken);
        }

        private async void ProcessDeadLetters(string route, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var topics = _messageTypesCache.GetAllTopics();
                    foreach (var topic in topics)
                    {
                        var letters = await _deadLettersAgent.GetDeadLetters(route, topic, cancellationToken);
                        foreach (var letter in letters)
                        {
                            await _publishingAgent.PublishToSubscriberAsync(route, topic, letter.Payload);
                        }
                    }
                    return;
                }
                catch (Exception e)
                {
                    _messagingLogger.Error(e, $"deadletters processing in {route}");
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                }
            }
        }

        private async void RunListener(string route, CancellationToken cancellationToken)
        {
            try
            {
                _unprocessedMessagesResender.Start(route, _messagingConfiguration.ResendInterval, cancellationToken);
                while (true)
                {
                    try
                    {
                        await _listeningAgent.Listen(route, cancellationToken);
                    }
                    catch (TaskCanceledException)
                    {
                        _messagingLogger.Info($"{route} cancelled listener");
                        return;
                    }

                    catch (Exception e)
                    {
                        _messagingLogger.Error(e, $"error on listening in {route}, trying againg");
                    }
                    await Task.Delay(500, cancellationToken);
                }
            }
            catch (Exception e)
            {
                _messagingLogger.Error(e, $"error on starting listening in {route}, listener is stopped");
            }
        }
    }
}