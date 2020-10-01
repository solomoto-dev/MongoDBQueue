using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MongoQueue.Core.AgentAbstractions;
using MongoQueue.Core.Entities;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.LogicAbstractions;
using Newtonsoft.Json;

namespace MongoQueue.Core.Logic
{
    public class QueuePublisher : IQueuePublisher
    {
        private readonly ITopicNameProvider _topicNameProvider;
        private readonly IMessagingLogger _messagingLogger;
        private readonly IPublishingAgent _publishingAgent;
        private readonly ISubscriptionAgent _subscriptionAgent;
        private readonly IDeadLettersAgent _deadLettersAgent;

        public QueuePublisher(
            ITopicNameProvider topicNameProvider,
            IMessagingLogger messagingLogger,
            IPublishingAgent publishingAgent,
            ISubscriptionAgent subscriptionAgent, IDeadLettersAgent deadLettersAgent)
        {
            _topicNameProvider = topicNameProvider;
            _messagingLogger = messagingLogger;
            _publishingAgent = publishingAgent;
            _subscriptionAgent = subscriptionAgent;
            _deadLettersAgent = deadLettersAgent;
        }

        ///<inheritdoc/>
        public void Publish<TMessage>(TMessage message, Ack ack = Ack.Master)
        {
            var topic = _topicNameProvider.Get<TMessage>();
            Publish(topic, message, ack);
        }

        ///<inheritdoc/>
        public async Task PublishAsync<TMessage>(TMessage message, Ack ack = Ack.Master)
        {
            var topic = _topicNameProvider.Get<TMessage>();
            await PublishAsync(topic, message, ack);
        }

        ///<inheritdoc/>
        public async Task PublishAsync(string topic, object message, Ack ack = Ack.Master)
        {
            var sw = Stopwatch.StartNew();

            var subscribers = await _subscriptionAgent.GetSubscribersAsync(topic);

            var payload = JsonConvert.SerializeObject(message);
            if (subscribers != null && subscribers.Any())
            {
                foreach (var subscriber in subscribers)
                {
                    await _publishingAgent.PublishToSubscriberAsync(subscriber.Name, topic, payload, ack);
                }
            }
            else
            {
                _messagingLogger.Debug($"no subsriptions for {topic}");
                await _deadLettersAgent.PublishAsync(topic, payload, ack);
            }
            _messagingLogger.Debug($"{topic} sent in {sw.ElapsedMilliseconds}");
        }

        ///<inheritdoc/>
        public void Publish(string topic, object message, Ack ack = Ack.Master)
        {
            var sw = Stopwatch.StartNew();
            var payload = JsonConvert.SerializeObject(message);

            var subscribers = _subscriptionAgent.GetSubscribers(topic);            
            if (subscribers != null && subscribers.Any())
            {
                foreach (var subscriber in subscribers)
                {
                    _publishingAgent.PublishToSubscriber(subscriber.Name, topic, payload, ack);
                }
            }
            else
            {
                _messagingLogger.Debug($"no subsriptions for {topic}");
                _deadLettersAgent.Publish(topic, payload, ack);
            }
            _messagingLogger.Debug($"{topic} sent in {sw.ElapsedMilliseconds}");
        }
    }
}