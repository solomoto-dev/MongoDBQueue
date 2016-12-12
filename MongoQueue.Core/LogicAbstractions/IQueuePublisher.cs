using System.Threading.Tasks;

namespace MongoQueue.Core.LogicAbstractions
{
    public interface IQueuePublisher
    {
        void Publish<TMessage>(TMessage message);
        void Publish(string topic, object message);
        Task PublishAsync<TMessage>(TMessage message);
        Task PublishAsync(string topic, object message);
    }
}