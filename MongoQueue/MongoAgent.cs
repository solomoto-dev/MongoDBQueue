using MongoDB.Driver;
using MongoQueue.Core.Entities;
using MongoQueue.Core.IntegrationAbstractions;

namespace MongoQueue
{
    public class MongoAgent
    {
        private readonly IMessagingConfiguration _messagingConfiguration;
        private readonly string _dbName;

        public MongoAgent(
            IMessagingConfiguration messagingConfiguration
        )
        {
            _messagingConfiguration = messagingConfiguration;
            _dbName = _messagingConfiguration.Database;
        }

        public IMongoDatabase GetDb()
        {
            var mongoUrl = MongoUrl.Create(_messagingConfiguration.ConnectionString);
            var settings = MongoClientSettings.FromUrl(mongoUrl);
            settings.WriteConcern = WriteConcern.Acknowledged;
            var client = new MongoClient(settings);
            return client.GetDatabase(_dbName);
        }

        public IMongoCollection<Subscriber> GetSubscribers()
        {
            return GetCollection<Subscriber>();
        }

        private IMongoCollection<TDocument> GetCollection<TDocument>(string name = null)
        {
            name = name ?? typeof(TDocument).Name;
            var db = GetDb();
            return db.GetCollection<TDocument>(name, new MongoCollectionSettings
            {
                WriteConcern = WriteConcern.Acknowledged,
                ReadConcern = ReadConcern.Default
            });
        }

        public IMongoCollection<Envelope> GetEnvelops(string route)
        {
            return GetCollection<Envelope>(GetEnvelopsCollectionName(route));
        }

        public string GetEnvelopsCollectionName(string route)
        {
            return $"{route}_Envelops";
        }
    }
}