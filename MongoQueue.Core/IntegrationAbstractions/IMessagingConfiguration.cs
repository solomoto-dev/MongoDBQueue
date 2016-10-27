using System;

namespace MongoQueue.Core.IntegrationAbstractions
{
    public interface IMessagingConfiguration
    {
        string ConnectionString { get; }
        TimeSpan ResendInterval { get; }
        TimeSpan ResendTreshold { get; }
    }
}