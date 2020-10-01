using System;
using System.Net.Sockets;
using MongoDB.Driver;
using MongoQueue.Core.Entities;
using MongoQueue.Core.Exceptions;
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
            _dbName = _messagingConfiguration.Database;
            _db = new Lazy<MongoDatabase>(GetDb);
        }

        private readonly Lazy<MongoDatabase> _db;
        public MongoDatabase Db => _db.Value;

        private MongoDatabase GetDb()
        {
            var mongoUrl = MongoUrl.Create(_messagingConfiguration.ConnectionString);
            var settings = MongoClientSettings.FromUrl(mongoUrl);
            settings.WriteConcern = WriteConcern.Acknowledged;
            var client = new MongoClient(settings);
            var server = client.GetServer();
            Ping(server);
            return server.GetDatabase(_dbName);
        }

        private static void Ping(MongoServer server)
        {
            try
            {
                server.Ping();
            }
            catch (SocketException)
            {
                throw new QueueConfigurationException();
            }
        }

        public MongoCollection<Subscriber> GetSubscribers()
        {
            return GetCollection<Subscriber>();
        }

        public MongoCollection<TDocument> GetCollection<TDocument>(string name = null)
        {
            name = name ?? typeof(TDocument).Name;
            var db = Db;
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
