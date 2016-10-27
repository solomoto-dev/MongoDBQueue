using System.Threading.Tasks;
using MongoQueue.Core.AgentAbstractions;
using MongoQueue.Core.Entities;

namespace MongoQueue.Legacy
{
    public class LegacyPublishingAgent : IPublishingAgent
    {
        private readonly LegacyMongoAgent _mongoAgent;

        public LegacyPublishingAgent(LegacyMongoAgent mongoAgent)
        {
            _mongoAgent = mongoAgent;
        }
        public void PublishToSubscriber(string subscriberName, string topic, string payload)
        {
            var collection = _mongoAgent.GetEnvelops(subscriberName);
            collection.Insert(new Envelope(topic, payload));
        }

        public async Task PublishToSubscriberAsync(string subscriberName, string topic, string payload)
        {
            PublishToSubscriber(subscriberName, topic, payload);
        }
    }
}