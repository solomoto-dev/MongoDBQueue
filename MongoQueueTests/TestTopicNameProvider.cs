using MongoQueueShared;
using MongoQueueShared.Common;

namespace MongoQueueTests
{
    public class TestTopicNameProvider : TopicNameProvider
    {
        public override string Get<TMessage>()
        {
            if (typeof(TMessage) == typeof(TestMessage) || typeof(TMessage) == typeof(SlightlyDifferentTestMessage))
            {
                return nameof(TestMessage);
            }
            return base.Get<TMessage>();
        }
    }
}