using System;
using MongoQueue.Core.Logic;

namespace MongoQueueTests.Common
{
    public class TestTopicNameProvider : DefaultTopicNameProvider
    {
        public override string Get(Type message)
        {
            if (message == typeof(TestMessage) || message == typeof(SlightlyDifferentTestMessage))
            {
                return nameof(TestMessage);
            }
            return base.Get(message);
        }
    }
}