using System;

namespace MongoQueue.Core.Entities
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
        public bool IsRead { get; private set; }
        public bool IsProcessingStarted { get; private set; }
        public bool IsProcessed { get; private set; }


        internal Envelope()
        {
            CreatedAt = DateTime.UtcNow;
            ReadAt = DateTime.MinValue;
            ProcessingStartedAt = DateTime.MinValue;
            ProcessedAt = DateTime.MinValue;
            Id = IdGenerator.GetId();
            ResendId = IdGenerator.GetId();
        }

        public Envelope(string topic, string payload, string originalId = null) : this()
        {
            Topic = topic;
            Payload = payload;
            OriginalId = originalId ?? IdGenerator.Empty;
        }
    }
}