using System;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using MongoDB.Driver.Core.WireProtocol.Messages.Encoders;
using MongoQueue.Core.Entities;
using MongoQueue.Core.Exceptions;
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
            _db = new Lazy<IMongoDatabase>(GetDb);
        }

        private readonly Lazy<IMongoDatabase> _db;
        public IMongoDatabase Db => _db.Value;
        private IMongoDatabase GetDb()
        {
            var mongoUrl = MongoUrl.Create(_messagingConfiguration.ConnectionString);
            var settings = MongoClientSettings.FromUrl(mongoUrl);
            settings.WriteConcern = WriteConcern.Acknowledged;
            var client = new MongoClient(settings);
            var database = client.GetDatabase(_dbName);
            var res = Ping(database);
            if(!res) throw new QueueConfigurationException();
            return database;
        }

        private static bool Ping(IMongoDatabase database)
        {
            try
            {                
                var res = database.RunCommand(new ObjectCommand<BsonDocument>(new {ping = 1}));
                return (double)res.ToDictionary()["ok"] == 1.0;
            }
            catch (TimeoutException)
            {
                return false;
            }
        }

        public IMongoCollection<Subscriber> GetSubscribers()
        {
            return GetCollection<Subscriber>();
        }

        public IMongoCollection<TDocument> GetCollection<TDocument>(string name = null)
        {
            name = name ?? typeof(TDocument).Name;
            return Db.GetCollection<TDocument>(name, new MongoCollectionSettings
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