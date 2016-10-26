using System;

namespace MongoQueueShared.Read
{
    public interface IMessagingConfiguration
    {
        string ConnectionString { get; }
        TimeSpan ResendInterval { get; }
        TimeSpan ResendTreshold { get; }
    }
}