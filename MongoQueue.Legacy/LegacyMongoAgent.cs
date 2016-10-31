using MongoDB.Driver;
using MongoQueue.Core.Entities;
using MongoQueue.Core.IntegrationAbstractions;

namespace MongoQueue.Legacy
{
    public class LegacyMongoAgent
    {
        private readonly IMessagingConfiguration _messagingConfiguration;
        private readonly string _dbName;

        public LegacyMongoAgent(IMessagingConfiguration messagingConfiguration)
        {
            _messagingConfiguration = messagingConfiguration;
            _dbName = MongoUrl.Create(_messagingConfiguration.ConnectionString).DatabaseName;
        }
        public MongoDatabase GetDb()
        {
            var mongoUrl = MongoUrl.Create(_messagingConfiguration.ConnectionString);
            var settings = MongoClientSettings.FromUrl(mongoUrl);
            settings.WriteConcern = WriteConcern.Acknowledged;
            var client = new MongoClient(settings);
            return client.GetServer().GetDatabase(_dbName);
        }

        public MongoCollection<Subscriber> GetSubscribers()
        {
            return GetCollection<Subscriber>();
        }

        private MongoCollection<TDocument> GetCollection<TDocument>(string name = null)
        {
            name = name ?? typeof(TDocument).Name;
            var db = GetDb();
            return db.GetCollection<TDocument>(name, new MongoCollectionSettings
            {
                WriteConcern = WriteConcern.Acknowledged
            });
        }

        public MongoCollection<Envelope> GetEnvelops(string route)
        {
            return GetCollection<Envelope>(GetEnvelopsCollectionName(route));
        }

        public string GetEnvelopsCollectionName(string route)
        {
            return $"{route}_Envelops";
        }
    }
}
