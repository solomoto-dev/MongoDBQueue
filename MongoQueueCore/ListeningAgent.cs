using System;
using System.Linq;
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

        public async Task Listen(string route, CancellationToken cancellationToken)
        {
            var notReadFilter = Builders<Envelope>.Filter.Eq(x => x.IsRead, false);
            var collection = _mongoAgent.GetEnvelops(route);
            try
            {
                while (true)
                {
                    var messages = await (await collection.FindAsync(notReadFilter, null, cancellationToken)).ToListAsync(cancellationToken);
                    {
                        foreach (var message in messages)
                        {
                            var readMessage = await _messageStatusManager.TrySetReadAt(route, message.Id, cancellationToken);
                            if (readMessage != null)
                            {
                                var resend = readMessage.OriginalId != IdGenerator.Empty;
                                _messageProcessor.Process(route, readMessage.Id, readMessage.Topic, readMessage.Payload, resend, cancellationToken);
                            }
                        }
                    }
                    if (!messages.Any())
                    {
                        await Task.Delay(100, cancellationToken);
                    }
                }
            }
            catch (MongoCommandException mongoCommandException)
            {
                if (mongoCommandException.Code == 96)
                {
                    _messagingLogger.Error(mongoCommandException,
                        $"{route} reader processes messages slower than they occur");
                }
                else
                {
                    _messagingLogger.Error(mongoCommandException, $"{route}");
                }
            }
        }
    }
}