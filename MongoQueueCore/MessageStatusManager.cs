using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoQueue.Core.AgentAbstractions;
using MongoQueue.Core.Entities;

namespace MongoQueue
{
    public class MessageStatusManager : IMessageStatusManager
    {
        private readonly MongoAgent _mongoAgent;

        public MessageStatusManager(MongoAgent mongoAgent)
        {
            _mongoAgent = mongoAgent;
        }

        public async Task<bool> TrySetProcessingStartedAt(string route, string messageId, CancellationToken cancellationToken)
        {
            var collection = _mongoAgent.GetEnvelops(route);
            var filter = Builders<Envelope>.Filter.And(Builders<Envelope>.Filter.Eq(x => x.Id, messageId),
                Builders<Envelope>.Filter.Eq(x => x.IsProcessed, false));
            var found = await collection.FindOneAndUpdateAsync<Envelope>(filter,
                Builders<Envelope>.Update.Set(x => x.ProcessingStartedAt, DateTime.UtcNow).Set(x => x.IsProcessingStarted, true),
                cancellationToken: cancellationToken);
            return found != null;
        }

        public async Task SetProcessedAt(string route, string messageId, CancellationToken cancellationToken)
        {
            var collection = _mongoAgent.GetEnvelops(route);
            await collection.FindOneAndUpdateAsync<Envelope>(Builders<Envelope>.Filter.Eq(x => x.Id, messageId),
                Builders<Envelope>.Update.Set(x => x.ProcessedAt, DateTime.UtcNow).Set(x => x.IsProcessed, true),
                cancellationToken: cancellationToken);
        }

        public async Task<Envelope> TrySetReadAt(string route, string messageId, CancellationToken cancellationToken)
        {
            var notReadAndIdQuery =
                            Builders<Envelope>.Filter.And(
                                    Builders<Envelope>.Filter.Eq(x => x.Id, messageId),
                                    Builders<Envelope>.Filter.Eq(x => x.IsRead, false)
                                );
            var update = Builders<Envelope>.Update.Set(x => x.ReadAt, DateTime.UtcNow)
                .Set(x => x.IsRead, true);
            var collection = _mongoAgent.GetEnvelops(route);
            var message = await collection.FindOneAndUpdateAsync(notReadAndIdQuery, update,
                cancellationToken: cancellationToken);
            return message;
        }
    }
}