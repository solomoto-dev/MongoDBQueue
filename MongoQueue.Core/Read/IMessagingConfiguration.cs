using System;

namespace MongoQueue.Core.Read
{
    public interface IMessagingConfiguration
    {
        string ConnectionString { get; }
        TimeSpan ResendInterval { get; }
        TimeSpan ResendTreshold { get; }
    }
}