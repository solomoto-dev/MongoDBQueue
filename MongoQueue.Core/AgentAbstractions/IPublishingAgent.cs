using System.Threading.Tasks;
using MongoQueue.Core.Entities;

namespace MongoQueue.Core.AgentAbstractions
{
    public interface IPublishingAgent
    {
        void PublishToSubscriber(string subscriberName,string topic, string payload, Ack ack = Ack.Master);
        Task PublishToSubscriberAsync(string subscriberName, string topic, string payload, Ack ack = Ack.Master);
    }
}