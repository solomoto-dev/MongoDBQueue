using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core.Entities;

namespace MongoQueue.Core.AgentAbstractions
{
    public interface IUnprocessedMessagesAgent
    {
        Task<List<Envelope>> GetUnprocessed(string route, CancellationToken cancellationToken);
        Task<string> Resend(string route, Envelope original, CancellationToken cancellationToken);
    }
}