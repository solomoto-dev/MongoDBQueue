using System.Threading.Tasks;
using MongoDB.Driver.Builders;
using MongoQueue.Core.AgentAbstractions;
using MongoQueue.Core.Entities;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Legacy
{
    public class LegacyCollectionCreator : ICollectionCreator
    {
        private readonly LegacyMongoAgent _mongoAgent;

        public LegacyCollectionCreator(LegacyMongoAgent mongoAgent)
        {
            _mongoAgent = mongoAgent;
        }
        public Task CreateCollectionIfNotExist(string route)
        {
            var collectionName = _mongoAgent.GetEnvelopsCollectionName(route);
            var db = _mongoAgent.GetDb();
            if (!db.CollectionExists(collectionName))
            {
                var collectionOptionsBuilder = new CollectionOptionsBuilder()
                    .SetCapped(true)
                    .SetMaxDocuments(100000)
                    .SetMaxSize(300000000);
                db.CreateCollection(collectionName, collectionOptionsBuilder);
                var collection = db.GetCollection(collectionName);
                collection.CreateIndex(IndexKeys<Envelope>.Ascending(x => x.IsRead));
                collection.CreateIndex(IndexKeys<Envelope>.Descending(x=>x.ReadAt).Ascending(x => x.ProcessedAt));
                collection.CreateIndex(IndexKeys<Envelope>.Descending(x=>x.ProcessingStartedAt).Ascending(x => x.ProcessedAt));
                collection.CreateIndex(IndexKeys<Envelope>.Descending(x=>x.Id).Ascending(x => x.ProcessedAt));
            }

            return Task.FromResult(true);
        }
    }
}