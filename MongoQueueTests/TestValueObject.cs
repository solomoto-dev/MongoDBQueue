namespace MongoQueueTests
{
    public class TestValueObject
    {
        public string Id { get; }
        public string Name { get; }

        public TestValueObject(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}