using System.Threading.Tasks;

namespace MongoQueueShared.Common
{
    public interface IQueuePublisher
    {
        void Publish<TMessage>(TMessage message);
        Task PublishAsync<TMessage>(TMessage message);
    }
}