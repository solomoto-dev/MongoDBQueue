using MongoQueue.Core.Logic;

namespace MongoQueueTests.Common
{
    public class TestTopicNameProvider : DefaultTopicNameProvider
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