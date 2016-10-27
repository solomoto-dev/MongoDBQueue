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

        public async Task UpdateSubscriber(string appName, string[] topics)
        {
            var subscriber = new Subscriber(appName, topics);
            var subscribersCollection = _mongoAgent.GetSubscribers();
            var query = Query<Subscriber>.EQ(x => x.Name, appName);
            var existingSubscriber = subscribersCollection.Find(query).FirstOrDefault();
            if (existingSubscriber != null)
            {
                subscribersCollection.Update(query, Update<Subscriber>.Set(x => x.Topics, topics));
            }
            else
            {
                subscribersCollection.Insert(subscriber);
            }
        }
    }
}