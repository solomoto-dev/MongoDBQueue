using Microsoft.Extensions.DependencyInjection;

namespace MongoQueue.Core
{
    public interface IMessagingDependencyRegistrator
    {
        void RegisterDefault(IServiceCollection registrator);
    }
}