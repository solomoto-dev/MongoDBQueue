using System.Threading.Tasks;

namespace MongoQueue.Core.LogicAbstractions
{
    public interface IQueuePublisher
    {
        void Publish<TMessage>(TMessage message);
        Task PublishAsync<TMessage>(TMessage message);
    }
}