using MongoQueue.Core.IntegrationAbstractions;
using System;

namespace MongoQueue.Core.IntegrationDefaults
{
    public class DefaultMessagingConfiguration : IMessagingConfiguration
    {
        public string ConnectionString { get; }
        public string Database { get; }
        public TimeSpan ResendInterval { get; protected set; }
        public TimeSpan ResendThreshold { get; protected set; }
        public CursorType CursorType { get; }
        public int MaxResendsThreshold { get; }
        public DefaultMessagingConfiguration(string connectionString, string database, TimeSpan resendInterval, TimeSpan resendThreshold, CursorType cursorType, int maxResendsThreshold)
        {
            ConnectionString = connectionString;
            Database = database;
            ResendInterval = resendInterval;
            ResendThreshold = resendThreshold;
            CursorType = cursorType;
            MaxResendsThreshold = maxResendsThreshold;
        }

        public static DefaultMessagingConfiguration Create()
        {
            return Create("mongodb://localhost:27017", "dev-queue");
        }

        public static DefaultMessagingConfiguration Create(string connectionString, string database)
        {
            return new DefaultMessagingConfiguration(connectionString, database, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), CursorType.Polling, 10);
        }
    }
}