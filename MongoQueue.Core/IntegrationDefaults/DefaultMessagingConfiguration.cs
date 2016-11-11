using System;
using MongoQueue.Core.IntegrationAbstractions;

namespace MongoQueue.Core.IntegrationDefaults
{
    public class DefaultMessagingConfiguration : IMessagingConfiguration
    {
        public string ConnectionString { get; }
        public string Database { get; }
        public TimeSpan ResendInterval { get; set; }
        public TimeSpan ResendTreshold { get; set; }

        public DefaultMessagingConfiguration(string connectionString, string database, TimeSpan resendInterval, TimeSpan resendTreshold)
        {
            ConnectionString = connectionString;
            Database = database;
            ResendInterval = resendInterval;
            ResendTreshold = resendTreshold;
        }
    }
}