using System;

namespace MongoQueueTests
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