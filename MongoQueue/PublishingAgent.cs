using System.Threading.Tasks;
using MongoQueue.Core.AgentAbstractions;
using MongoQueue.Core.Entities;

namespace MongoQueue
{
    public class PublishingAgent : IPublishingAgent
    {
        private readonly MongoAgent _mongoAgent;

        public PublishingAgent(MongoAgent mongoAgent)
        {
            _mongoAgent = mongoAgent;
        }

        public void PublishToSubscriber(string subscriberName,string topic, string payload, Ack ack = Ack.Master)
        {
            var collection = _mongoAgent
                .GetEnvelops(subscriberName)
                .WithWriteConcern(ack.ToWriteConcern());

            collection.InsertOne(new Envelope(topic, payload));
        }

        public async Task PublishToSubscriberAsync(string subscriberName, string topic, string payload, Ack ack = Ack.Master)
        {
            var collection = _mongoAgent
                .GetEnvelops(subscriberName)
                .WithWriteConcern(ack.ToWriteConcern());

            await collection.InsertOneAsync(new Envelope(topic, payload));
        }
    }
}