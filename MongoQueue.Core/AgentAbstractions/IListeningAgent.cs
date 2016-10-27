using System.Threading;
using System.Threading.Tasks;

namespace MongoQueue.Core.AgentAbstractions
{
    public interface IListeningAgent
    {
        Task Listen(string appName, CancellationToken cancellationToken);
    }
}