using System.Threading.Tasks;

namespace MongoQueue.Core.Common
{
    public interface IQueuePublisher
    {
        void Publish<TMessage>(TMessage message);
        Task PublishAsync<TMessage>(TMessage message);
    }
}