using System.Collections.Generic;
using System.Threading.Tasks;
using MongoQueue.Core.Entities;

namespace MongoQueue.Core.AgentAbstractions
{
    public interface ISubscriptionAgent
    {
        List<Subscriber> GetSubscribers(string topic);
        Task<List<Subscriber>> GetSubscribersAsync(string topic);
        Task UpdateSubscriber(string route, string[] topics);
    }
}