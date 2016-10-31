using System.Threading.Tasks;

namespace MongoQueue.Core.AgentAbstractions
{
    public interface ICollectionCreator
    {
        Task CreateCollectionIfNotExist(string route);
    }
}