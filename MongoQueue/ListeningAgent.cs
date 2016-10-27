using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoQueue.Core;
using MongoQueue.Core.AgentAbstractions;
using MongoQueue.Core.Entities;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.Logic;

namespace MongoQueue
{
    public class ListeningAgent : IListeningAgent
    {
        private readonly MongoAgent _mongoAgent;
        private readonly IMessagingLogger _messagingLogger;
        private readonly IMessageStatusManager _messageStatusManager;
        private readonly MessageProcessor _messageProcessor;

        public ListeningAgent(
            MongoAgent mongoAgent,
            IMessagingLogger messagingLogger,
            IMessageStatusManager messageStatusManager,
            MessageProcessor messageProcessor
        )
        {
            _mongoAgent = mongoAgent;
            _messagingLogger = messagingLogger;
            _messageStatusManager = messageStatusManager;
            _messageProcessor = messageProcessor;
        }

        public async Task Listen(string appName, CancellationToken cancellationToken)
        {
            var findOptions = new FindOptions<Envelope>
            {
                CursorType = CursorType.TailableAwait,
                MaxAwaitTime = TimeSpan.FromMinutes(60),
                NoCursorTimeout = true,
                Sort = Builders<Envelope>.Sort.Ascending("$natural")
            };
            var notReadFilter = Builders<Envelope>.Filter.Eq(x => x.IsRead, false);
            var collection = _mongoAgent.GetEnvelops(appName);
            try
            {
                using (var cursor = await collection.FindAsync(notReadFilter, findOptions, cancellationToken))
                {
                    while (await cursor.MoveNextAsync(cancellationToken))
                    {
                        foreach (var message in cursor.Current)
                        {
                            var readMessage = await _messageStatusManager.TrySetReadAt(appName, message.Id, cancellationToken);
                            if (readMessage != null)
                            {
                                var resend = readMessage.OriginalId != IdGenerator.Empty;
                                _messageProcessor.Process(appName, readMessage.Id, readMessage.Topic, readMessage.Payload, resend, cancellationToken);
                            }
                        }
                    }
                    _messagingLogger.Debug($"{appName} cursor is dead");
                }
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
        }
    }
}