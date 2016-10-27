using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core.Entities;

namespace MongoQueue.Core.AgentAbstractions
{
    public interface IMessageStatusManager
    {
        Task<bool> TrySetProcessingStartedAt(string appName, string messageId, CancellationToken cancellationToken);
        Task SetProcessedAt(string appName, string messageId, CancellationToken cancellationToken);
        Task<Envelope> TrySetReadAt(string appName, string messageId, CancellationToken cancellationToken);
    }
}