using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoQueue.Core.Common;

namespace MongoQueue.Core.Read
{
    public class MongoMessageListener
    {
        private readonly IMessageTypesCache _messageTypesCache;
        private readonly MongoMessagingAgent _mongoMessagingAgent;
        private readonly IMessagingLogger _messagingLogger;
        private readonly MessageProcessor _messageProcessor;
        private readonly UnprocessedMessagesResender _unprocessedMessagesResender;

        public MongoMessageListener(
            IMessageTypesCache messageTypesCache,
            MongoMessagingAgent mongoMessagingAgent,
            IMessagingLogger messagingLogger,
            MessageProcessor messageProcessor,
            UnprocessedMessagesResender unprocessedMessagesResender
        )
        {
            _messageTypesCache = messageTypesCache;
            _mongoMessagingAgent = mongoMessagingAgent;
            _messagingLogger = messagingLogger;
            _messageProcessor = messageProcessor;
            _unprocessedMessagesResender = unprocessedMessagesResender;
        }

        public async Task Start(string appName, CancellationToken cancellationToken)
        {
            DocumentMappingInitializer.Initialize();
            if (appName.Contains(" "))
            {
                throw new ArgumentException("appName");
            }
            await UpdateSubscriber(appName);

            var collectionName = _mongoMessagingAgent.GetEnvelopsCollectionName(appName);
            await CreateCollectionIfNotExist(collectionName);

            AfterInit(appName, cancellationToken);
        }

        private async void AfterInit(string appName, CancellationToken cancellationToken)
        {
            try
            {
                _unprocessedMessagesResender.Start(appName, cancellationToken);
                while (true)
                {
                    try
                    {
                        await Listen(appName, cancellationToken);
                    }
                    catch (TaskCanceledException)
                    {
                        _messagingLogger.Info($"{appName} cancelled listener");
                        return;
                    }
                    catch (MongoCommandException mongoCommandException)
                    {
                        if (mongoCommandException.Code == 96)
                        {
                            _messagingLogger.Error(mongoCommandException,
                                $"{appName} reader processes messages slower than they occur");
                        }
                        else
                        {
                            _messagingLogger.Error(mongoCommandException, $"{appName}");
                        }
                    }
                    catch (Exception e)
                    {
                        _messagingLogger.Error(e, $"error on listening in {appName}, trying againg");
                    }
                    await Task.Delay(500, cancellationToken);
                }
            }
            catch (Exception e)
            {
                _messagingLogger.Error(e, $"error on starting listening in {appName}, listener is stopped");
            }
        }

        private async Task Listen(string appName, CancellationToken cancellationToken)
        {
            var findOptions = new FindOptions<Envelope>
            {
                CursorType = CursorType.TailableAwait,
                MaxAwaitTime = TimeSpan.FromMinutes(60),
                NoCursorTimeout = true,
                Sort = Builders<Envelope>.Sort.Ascending("$natural")
            };
            var notReadFilter = Builders<Envelope>.Filter.Eq(x => x.IsRead, false);

            var collection = _mongoMessagingAgent.GetEnvelops(appName);
            using (var cursor = await collection.FindAsync(notReadFilter, findOptions, cancellationToken))
            {
                while (await cursor.MoveNextAsync(cancellationToken))
                {
                    foreach (var document in cursor.Current)
                    {
                        var notReadAndIdQuery =
                            Builders<Envelope>.Filter.And(Builders<Envelope>.Filter.Eq("_id", document.Id),
                                notReadFilter);
                        var update = Builders<Envelope>.Update.Set(x => x.ReadAt, DateTime.UtcNow)
                            .Set(x => x.IsRead, true);
                        var message = collection.FindOneAndUpdate(notReadAndIdQuery, update,
                            cancellationToken: cancellationToken);
                        if (message != null)
                        {
                            var resend = message.OriginalId != ObjectId.Empty.ToString();
                            _messageProcessor.Process(appName, message.Id, message.Topic, message.Payload, resend,
                                cancellationToken);
                        }
                    }
                }
                _messagingLogger.Debug($"{appName} cursor is dead");
            }
        }

        private async Task UpdateSubscriber(string appName)
        {
            var topics = _messageTypesCache.GetAllTopics();
            var subscriber = new Subscriber(appName, topics);
            var subscribers = _mongoMessagingAgent.GetCollection<Subscriber>();
            var nameFilter = Builders<Subscriber>.Filter.Eq(x => x.Name, appName);
            var existingSubscriber = await (await subscribers.FindAsync(nameFilter)).FirstOrDefaultAsync();

            if (existingSubscriber != null)
            {
                await subscribers.UpdateOneAsync(nameFilter, Builders<Subscriber>.Update.Set(x => x.Topics, topics));
            }
            else
            {
                await subscribers.InsertOneAsync(subscriber);
            }
        }

        private async Task CreateCollectionIfNotExist(string collectionName)
        {
            var db = _mongoMessagingAgent.GetDb();
            var colfilter = new BsonDocument("name", collectionName);
            var collections = await db.ListCollectionsAsync(new ListCollectionsOptions {Filter = colfilter});
            if (!collections.Any())
            {
                db.CreateCollection(collectionName, new CreateCollectionOptions
                {
                    Capped = true,
                    MaxDocuments = 100000,
                    MaxSize = 300000000
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

                await mongoIndexManager.CreateOneAsync(isReadIndex, new CreateIndexOptions
                {
                    Name = nameof(isReadIndex)
                });

                await mongoIndexManager.CreateOneAsync(processingStartedAtIndex, new CreateIndexOptions
                {
                    Name = nameof(processingStartedAtIndex)
                });

                await mongoIndexManager.CreateOneAsync(readAtIndex, new CreateIndexOptions
                {
                    Name = nameof(readAtIndex)
                });

                await mongoIndexManager.CreateOneAsync(idProcessedAtIndex, new CreateIndexOptions
                {
                    Name = nameof(idProcessedAtIndex)
                });
            }
        }
    }
}