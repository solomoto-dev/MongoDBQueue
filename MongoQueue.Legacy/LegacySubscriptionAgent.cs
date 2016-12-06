using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver.Builders;
using MongoQueue.Core.AgentAbstractions;
using MongoQueue.Core.Entities;

namespace MongoQueue.Legacy
{
    public class LegacySubscriptionAgent : ISubscriptionAgent
    {
        private readonly LegacyMongoAgent _mongoAgent;

        public LegacySubscriptionAgent(LegacyMongoAgent mongoAgent)
        {
            _mongoAgent = mongoAgent;
        }
        public List<Subscriber> GetSubscribers(string topic)
        {
            var subscribersCollection = _mongoAgent.GetSubscribers();
            var subscribers = subscribersCollection.Find(Query<Subscriber>.All(x => x.Topics, new[] { topic })).ToList();
            return subscribers;
        }

        public async Task<List<Subscriber>> GetSubscribersAsync(string topic)
        {
            return GetSubscribers(topic);
        }

        public async Task UpdateSubscriber(string route, string[] topics)
        {
            var subscriber = new Subscriber(route, topics);
            var subscribersCollection = _mongoAgent.GetSubscribers();
            var query = Query<Subscriber>.EQ(x => x.Name, route);
            var existingSubscriber = subscribersCollection.Find(query).FirstOrDefault();
            if (existingSubscriber != null)
            {
                subscriber.Id = existingSubscriber.Id;
                subscribersCollection.Update(query, Update<Subscriber>.Replace(subscriber));
            }
            else
            {
                subscribersCollection.Insert(subscriber);
            }
        }
    }
}