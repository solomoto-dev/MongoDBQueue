using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoQueue.Core.AgentAbstractions;
using MongoQueue.Core.Entities;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue
{
    public class CollectionCreator : ICollectionCreator
    {
        private readonly MongoAgent _mongoAgent;

        public CollectionCreator(MongoAgent mongoAgent)
        {
            _mongoAgent = mongoAgent;
        }
        public async Task CreateCollectionIfNotExist(string route)
        {
            var collectionName = _mongoAgent.GetEnvelopsCollectionName(route);
            var db = _mongoAgent.GetDb();
            var colfilter = new BsonDocument("name", collectionName);
            var collections = await db.ListCollectionsAsync(new ListCollectionsOptions { Filter = colfilter });
            if (!collections.Any())
            {
                db.CreateCollection(collectionName, new CreateCollectionOptions
                {
                    Capped = true,
                    MaxDocuments = 1000000,
                    MaxSize = 500000000
                });
                var mongoIndexManager = db.GetCollection<Envelope>(collectionName).Indexes;

                var isReadIndex = Builders<Envelope>.IndexKeys.Ascending(x => x.IsRead);

                var readAtIndex =
                    Builders<Envelope>.IndexKeys.Combine(Builders<Envelope>.IndexKeys.Descending(x => x.ReadAt),
                        Builders<Envelope>.IndexKeys.Ascending(x => x.ProcessedAt));
                var processingStartedAtIndex =
                    Builders<Envelope>.IndexKeys.Combine(
                        Builders<Envelope>.IndexKeys.Descending(x => x.ProcessingStartedAt),
                        Builders<Envelope>.IndexKeys.Ascending(x => x.ProcessedAt));

                var idProcessedAtIndex =
                    Builders<Envelope>.IndexKeys.Combine(Builders<Envelope>.IndexKeys.Descending(x => x.Id),
                        Builders<Envelope>.IndexKeys.Ascending(x => x.ProcessedAt));

                await mongoIndexManager.CreateOneAsync(isReadIndex);
                await mongoIndexManager.CreateOneAsync(processingStartedAtIndex);
                await mongoIndexManager.CreateOneAsync(readAtIndex);
                await mongoIndexManager.CreateOneAsync(idProcessedAtIndex);
            }
        }
    }
}