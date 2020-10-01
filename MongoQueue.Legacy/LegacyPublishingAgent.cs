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
        public void PublishToSubscriber(string subscriberName, string topic, string payload, Ack ack = Ack.Master)
        {
            var collection = _mongoAgent.GetEnvelops(subscriberName);
            collection.Insert(new Envelope(topic, payload), ack.ToWriteConcern());
        }

        public Task PublishToSubscriberAsync(string subscriberName, string topic, string payload, Ack ack = Ack.Master)
        {
            PublishToSubscriber(subscriberName, topic, payload, ack);
            return Task.CompletedTask;
        }
    }
}