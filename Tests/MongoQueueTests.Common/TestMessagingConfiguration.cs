using System;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.IntegrationDefaults;

namespace MongoQueueTests.Common
{
    public class TestMessagingConfiguration : DefaultMessagingConfiguration
    {
        public TestMessagingConfiguration(string connectionString, string database, TimeSpan resendInterval, TimeSpan resendThreshold, CursorType cursorType, int maxResendsThreshold) : base(connectionString, database, resendInterval, resendThreshold, cursorType, maxResendsThreshold)
        {
        }

        public void SetResends(TimeSpan resendInterval, TimeSpan resendThreshold)
        {
            ResendInterval = resendInterval;
            ResendThreshold = resendThreshold;
        }

        public static TestMessagingConfiguration Create()
        {
            return new TestMessagingConfiguration("mongodb://localhost:27017", "dev-queue", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), CursorType.Polling, 10);
        }
    }
}