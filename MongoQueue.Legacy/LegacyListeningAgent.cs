using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoQueue.Core;
using MongoQueue.Core.AgentAbstractions;
using MongoQueue.Core.Entities;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.Logic;

namespace MongoQueue.Legacy
{
    public class LegacyListeningAgent : IListeningAgent
    {
        private readonly LegacyMongoAgent _mongoAgent;
        private readonly IMessagingLogger _messagingLogger;
        private readonly IMessageStatusManager _messageStatusManager;
        private readonly MessageProcessor _messageProcessor;

        public LegacyListeningAgent(
            LegacyMongoAgent mongoAgent,
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
            var notReadQuery = Query<Envelope>.EQ(x => x.IsRead, false);
            var collection = _mongoAgent.GetEnvelops(appName);
            try
            {
                while (true)
                {
                    var messages = collection.Find(notReadQuery).ToList();
                    foreach (var envelope in messages)
                    {
                        var readMessage = await _messageStatusManager.TrySetReadAt(appName, envelope.Id, cancellationToken);
                        if (readMessage != null)
                        {
                            var resend = readMessage.OriginalId != IdGenerator.Empty;
                            _messageProcessor.Process(appName, readMessage.Id, readMessage.Topic, readMessage.Payload, resend, cancellationToken);
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