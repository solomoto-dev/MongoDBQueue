using System;
using MongoQueue.Core.IntegrationAbstractions;

namespace MongoQueue.Core.IntegrationDefaults
{
    public class DefaultMessagingConfiguration : IMessagingConfiguration
    {
        public string ConnectionString { get; }
        public string Database { get; }
        public TimeSpan ResendInterval { get; protected set; }
        public TimeSpan ResendThreshold { get; protected set; }
        public CursorType CursorType { get; }

        public DefaultMessagingConfiguration(string connectionString, string database, TimeSpan resendInterval, TimeSpan resendThreshold, CursorType cursorType)
        {
            ConnectionString = connectionString;
            Database = database;
            ResendInterval = resendInterval;
            ResendThreshold = resendThreshold;
            CursorType = cursorType;
        }

        public static DefaultMessagingConfiguration Create()
        {
            return new DefaultMessagingConfiguration("mongodb://localhost:27017", "dev-queue", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), CursorType.Polling);
        }
    }
}