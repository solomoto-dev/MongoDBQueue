using System.Threading.Tasks;

namespace MongoQueue.Core.AgentAbstractions
{
    public interface IPublishingAgent
    {
        void PublishToSubscriber(string subscriberName,string topic, string payload);
        Task PublishToSubscriberAsync(string subscriberName, string topic, string payload);
    }
}