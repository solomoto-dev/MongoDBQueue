using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver.Builders;
using MongoQueue.Core.AgentAbstractions;
using MongoQueue.Core.Entities;
using MongoQueue.Core.IntegrationAbstractions;

namespace MongoQueue.Legacy
{
    public class LegacyUnprocessedMessagesAgent : IUnprocessedMessagesAgent
    {
        private readonly LegacyMongoAgent _mongoAgent;
        private readonly IMessagingConfiguration _messagingConfiguration;
        private readonly IMessagingLogger _messagingLogger;

        public LegacyUnprocessedMessagesAgent(
            LegacyMongoAgent mongoAgent,
            IMessagingConfiguration messagingConfiguration,
            IMessagingLogger messagingLogger
        )
        {
            _mongoAgent = mongoAgent;
            _messagingConfiguration = messagingConfiguration;
            _messagingLogger = messagingLogger;
        }
        public async Task<List<Envelope>> GetUnprocessed(string route, CancellationToken cancellationToken)
        {
            var threshold = DateTime.UtcNow - _messagingConfiguration.ResendThreshold;
            var collection = _mongoAgent.GetEnvelops(route);
            var notProcessedQuery = Query.And(
                Query<Envelope>.LT(x => x.ReadAt, threshold),
                Query<Envelope>.EQ(x => x.IsRead, true),
                Query<Envelope>.EQ(x => x.IsProcessed, false)
            );
            var notProcessed = collection.Find(notProcessedQuery).ToList();
            _messagingLogger.Debug($"got {notProcessed.Count} unprocessed messages to resend");
            return notProcessed;
        }

        public async Task<string> Resend(string route, Envelope original, CancellationToken cancellationToken)
        {
            var collection = _mongoAgent.GetEnvelops(route);
            if (original.ResentCount >= _messagingConfiguration.MaxResendsThreshold)
            {
                collection.Update(Query<Envelope>.EQ(x => x.Id, original.Id),
                    Update<Envelope>
                        .Set(x => x.IsProcessed, true)
                        .Set(x => x.ProcessedAt, DateTime.UtcNow)
                        .Set(x => x.ResendId, original.Id)
                        );
                return original.Id;
            }

            var resend = new Envelope(original.Topic, original.Payload, original.Id, original.ResentCount + 1);

            collection.Insert(resend);
            var update = Update<Envelope>
                .Set(x => x.ProcessedAt, DateTime.UtcNow)
                .Set(x => x.IsProcessed, true)
                .Set(x => x.ResendId, resend.Id);
            collection.Update(Query<Envelope>.EQ(x => x.Id, original.Id), update);
            _messagingLogger.Debug($"resent to {route}  id {original.Id} newid {resend.Id} ");
            return resend.Id;
        }
    }
}