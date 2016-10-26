using System;
using MongoDB.Bson;

namespace MongoQueueShared.Common
{
    public class Envelope : IDocument
    {
        public string Id { get; set; }
        public string OriginalId { get; set; }
        public string Topic { get; private set; }
        public string Payload { get; private set; }
        public string ResendId { get; set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ReadAt { get; private set; }
        public DateTime ProcessingStartedAt { get; private set; }
        public DateTime ProcessedAt { get; private set; }

        internal Envelope()
        {
            CreatedAt = DateTime.UtcNow;
            ReadAt = DateTime.MinValue;
            ProcessingStartedAt = DateTime.MinValue;
            ProcessedAt = DateTime.MinValue;
            Id = ObjectId.GenerateNewId().ToString();
            ResendId = ObjectId.Empty.ToString();
        }

        public Envelope(string topic, string payload, string originalId = null) : this()
        {
            Topic = topic;
            Payload = payload;
            OriginalId = originalId ?? ObjectId.Empty.ToString();
        }
    }
}