using MongoDB.Bson;

namespace MongoQueueShared.Common
{
    public class Subscriber : IDocument
    {
        public string Name { get; private set; }
        public string[] Topics { get; private set; }
        public string Id { get; set; }

        public Subscriber(string name, string[] topics)
        {
            Name = name;
            Topics = topics;
            Id = ObjectId.GenerateNewId().ToString();
        }
    }
}