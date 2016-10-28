namespace MongoQueueTests.Common
{
    public class SlightlyDifferentTestMessage
    {
        public string Id { get; }
        public SlightlyDifferentTestValueObject TestValueObject { get; }

        public SlightlyDifferentTestMessage(string id, SlightlyDifferentTestValueObject testValueObject)
        {
            Id = id;
            TestValueObject = testValueObject;
        }
    }
}