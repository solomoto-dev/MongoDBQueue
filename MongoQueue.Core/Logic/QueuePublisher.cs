using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MongoQueue.Core.AgentAbstractions;
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

        public QueuePublisher(
            ITopicNameProvider topicNameProvider,
            IMessagingLogger messagingLogger,
            IPublishingAgent publishingAgent,
            ISubscriptionAgent subscriptionAgent
        )
        {
            _topicNameProvider = topicNameProvider;
            _messagingLogger = messagingLogger;
            _publishingAgent = publishingAgent;
            _subscriptionAgent = subscriptionAgent;
        }

        public void Publish<TMessage>(TMessage message)
        {
            var sw = Stopwatch.StartNew();
            var topic = _topicNameProvider.Get<TMessage>();

            var subscribers = _subscriptionAgent.GetSubscribers(topic);

            if (subscribers != null && subscribers.Any())
            {
                foreach (var subscriber in subscribers)
                {
                    var payload = JsonConvert.SerializeObject(message);
                    _publishingAgent.PublishToSubscriber(subscriber.Name, topic, payload);
                }
            }
            else
            {
                _messagingLogger.Debug($"no subsriptions for {topic}");
            }
            _messagingLogger.Trace($"{topic} sent in {sw.ElapsedMilliseconds}");
        }

        public async Task PublishAsync<TMessage>(TMessage message)
        {
            var sw = Stopwatch.StartNew();
            var topic = _topicNameProvider.Get<TMessage>();
            var subscribers = await _subscriptionAgent.GetSubscribersAsync(topic);

            if (subscribers != null)
            {
                foreach (var subscriber in subscribers)
                {
                    var payload = JsonConvert.SerializeObject(message);
                    await _publishingAgent.PublishToSubscriberAsync(subscriber.Name, topic, payload);
                }
            }
            else
            {
                _messagingLogger.Debug($"no subsriptions for {topic}");
            }
            _messagingLogger.Trace($"{topic} sent in {sw.ElapsedMilliseconds}");
        }
    }
}