using System.Diagnostics;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoQueue.Core.Common;
using Newtonsoft.Json;

namespace MongoQueue.Core.Write
{
    public class MongoQueuePublisher : IQueuePublisher
    {
        private readonly ITopicNameProvider _topicNameProvider;
        private readonly MongoMessagingAgent _mongoMessagingAgent;
        private readonly IMessagingLogger _messagingLogger;

        public MongoQueuePublisher(
            ITopicNameProvider topicNameProvider,
            MongoMessagingAgent mongoMessagingAgent,
            IMessagingLogger messagingLogger
            )
        {
            _topicNameProvider = topicNameProvider;
            _mongoMessagingAgent = mongoMessagingAgent;
            _messagingLogger = messagingLogger;
        }

        public void Publish<TMessage>(TMessage message)
        {
            var sw = Stopwatch.StartNew();
            var subscribersCollection = _mongoMessagingAgent.GetCollection<Subscriber>();
            var topic = _topicNameProvider.Get<TMessage>();
            var subscribers = subscribersCollection.Find(Builders<Subscriber>.Filter.All(x => x.Topics, new[] { topic })).ToList();

            if (subscribers != null)
            {
                foreach (var subscriber in subscribers)
                {
                    var collection = _mongoMessagingAgent.GetEnvelops(subscriber.Name);
                    var body = JsonConvert.SerializeObject(message);
                    collection.InsertOne(new Envelope(topic, body));
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
            var subscribersCollection = _mongoMessagingAgent.GetCollection<Subscriber>();
            var topic = _topicNameProvider.Get<TMessage>();
            var subscribers = await (await subscribersCollection.FindAsync(Builders<Subscriber>.Filter.All(x => x.Topics, new[] { topic }))).ToListAsync();

            if (subscribers != null)
            {
                foreach (var subscriber in subscribers)
                {
                    var collection = _mongoMessagingAgent.GetEnvelops(subscriber.Name);
                    var body = JsonConvert.SerializeObject(message);
                    await collection.InsertOneAsync(new Envelope(topic, body));
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