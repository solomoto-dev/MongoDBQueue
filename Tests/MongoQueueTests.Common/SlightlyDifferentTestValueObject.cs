using System;

namespace MongoQueueTests.Common
{
    public class SlightlyDifferentTestValueObject
    {
        public string Id { get; }
        public DateTime SomeDate { get; }

        public SlightlyDifferentTestValueObject(string id, DateTime someDate)
        {
            Id = id;
            SomeDate = someDate;
        }
    }
}