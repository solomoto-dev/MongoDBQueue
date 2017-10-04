using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core.Entities;

namespace MongoQueue.Core.AgentAbstractions
{
    public interface IDeadLettersAgent
    {
        Task<DeadLetter[]> GetDeadLetters(string route, string topic, CancellationToken cancellationToken);
        Task PublishAsync(string topic, string payload);
        void Publish(string topic, string payload);
    }
}