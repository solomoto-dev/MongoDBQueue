using System;

namespace MongoQueue.Core.Entities
{
    public class Subscriber : IDocument
    {
        public string Id { get; set; }
        public string Name { get; private set; }
        public string[] Topics { get; private set; }
        public DateTime SubscribedAt { get; private set; }

        public Subscriber(string name, string[] topics)
        {
            Name = name;
            Topics = topics;
            Id = IdGenerator.GetId();
            SubscribedAt = DateTime.UtcNow;
        }
    }
}