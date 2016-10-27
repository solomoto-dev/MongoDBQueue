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

        public LegacyUnprocessedMessagesAgent(
            LegacyMongoAgent mongoAgent, 
            IMessagingConfiguration messagingConfiguration
        )
        {
            _mongoAgent = mongoAgent;
            _messagingConfiguration = messagingConfiguration;
        }
        public async Task<List<Envelope>> GetUnprocessed(string appName, CancellationToken cancellationToken)
        {
            var treshold = DateTime.UtcNow - _messagingConfiguration.ResendTreshold;
            var collection = _mongoAgent.GetEnvelops(appName);
            var notProcessedQuery = Query.And(
                Query<Envelope>.LT(x => x.ReadAt, treshold),
                Query<Envelope>.EQ(x => x.IsRead, true),
                Query<Envelope>.EQ(x => x.IsProcessed, false)
            );
            var notProcessed = collection.Find(notProcessedQuery).ToList();
            return notProcessed;
        }

        public async Task<string> Resend(string appName, Envelope original, CancellationToken cancellationToken)
        {
            var collection = _mongoAgent.GetEnvelops(appName);
            var resend = new Envelope(original.Topic, original.Payload, original.Id);
            collection.Insert(resend);
            var update = Update<Envelope>
                .Set(x => x.ProcessedAt, DateTime.UtcNow)
                .Set(x => x.IsProcessed, true)
                .Set(x => x.ResendId, resend.Id);
            collection.Update(Query<Envelope>.EQ(x => x.Id, original.Id), update);
            return resend.Id;
        }
    }
}