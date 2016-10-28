using System;

namespace MongoQueueTests.Common
{
    public class AnotherTestMessage
    {
        public DateTime SomeDate { get; }
        public long? UselessNumber { get; }

        public AnotherTestMessage(DateTime someDate, long? uselessNumber)
        {
            SomeDate = someDate;
            UselessNumber = uselessNumber;
        }
    }
}