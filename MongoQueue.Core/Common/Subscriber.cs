using MongoDB.Bson;

namespace MongoQueue.Core.Common
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