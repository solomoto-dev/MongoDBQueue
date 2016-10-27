using System;
using MongoQueue.Core.IntegrationAbstractions;

namespace MongoQueue.Core.IntegrationDefaults
{
    public class DefaultMessagingConfiguration : IMessagingConfiguration
    {
        public string ConnectionString { get; }
        public TimeSpan ResendInterval { get; }
        public TimeSpan ResendTreshold { get; }

        public DefaultMessagingConfiguration(string connectionString, TimeSpan resendInterval, TimeSpan resendTreshold)
        {
            ConnectionString = connectionString;
            ResendInterval = resendInterval;
            ResendTreshold = resendTreshold;
        }
    }
}