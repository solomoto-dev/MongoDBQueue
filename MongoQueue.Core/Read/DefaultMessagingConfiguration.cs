using System;

namespace MongoQueue.Core.Read
{
    public class DefaultMessagingConfiguration : IMessagingConfiguration
    {
        public string ConnectionString { get; }
        public TimeSpan ResendInterval { get; }
        public TimeSpan ResendTreshold { get; }

        public DefaultMessagingConfiguration(string connectionString, TimeSpan resendInterval, TimeSpan resendTreshold)
        {
            ConnectionString = connectionString ?? "mongodb://localhost:27017/dev-queue";
            ResendInterval = resendInterval;
            ResendTreshold = resendTreshold;
        }
    }
}