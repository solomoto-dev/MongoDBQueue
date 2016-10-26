using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoQueue.Core.Common;

namespace MongoQueue.Core.Read
{
    public class UnprocessedMessagesResender
    {
        private readonly MongoMessagingAgent _mongoMessagingAgent;
        private readonly IMessagingConfiguration _messagingConfiguration;
        private readonly IMessagingLogger _messagingLogger;

        public UnprocessedMessagesResender(
            MongoMessagingAgent mongoMessagingAgent,
            IMessagingConfiguration messagingConfiguration,
            IMessagingLogger messagingLogger
            )
        {
            _mongoMessagingAgent = mongoMessagingAgent;
            _messagingConfiguration = messagingConfiguration;
            _messagingLogger = messagingLogger;
        }
        public async void Start(string appName, CancellationToken cancellationToken)
        {
            try
            {
                await RepeatEvery(() => Resend(appName, cancellationToken), _messagingConfiguration.ResendInterval);
            }
            catch (Exception e)
            {
                _messagingLogger.Error(e, $"{appName} error on UnprocessedMessagesResender.Start");
            }
        }

        private async Task RepeatEvery(Func<Task> action, TimeSpan interval)
        {
            while (true)
            {
                try
                {
                    await action();
                    await Task.Delay(interval);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
        }

        private async Task Resend(string appName, CancellationToken cancellationToken)
        {
            try
            {
                var collection = _mongoMessagingAgent.GetEnvelops(appName);
                var notProcessedFilter = Builders<Envelope>.Filter.And(
                    Builders<Envelope>.Filter.Lt(x => x.ReadAt, DateTime.UtcNow - _messagingConfiguration.ResendTreshold),
                    Builders<Envelope>.Filter.Eq(x => x.IsProcessed, false));

                var notProcessed =
                    await
                        (await collection.FindAsync(notProcessedFilter, cancellationToken: cancellationToken)).ToListAsync(
                            cancellationToken);
                foreach (var envelope in notProcessed)
                {
                    var resend = new Envelope(envelope.Topic, envelope.Payload, envelope.Id);
                    await collection.InsertOneAsync(resend, null, cancellationToken);
                    var updateDefinition = Builders<Envelope>.Update
                                                             .Set(x => x.ProcessedAt, DateTime.UtcNow)
                                                             .Set(x => x.IsProcessed, true)
                                                             .Set(x => x.ResendId, resend.Id);
                    await collection.UpdateOneAsync(Builders<Envelope>.Filter.Eq("_id", envelope.Id), updateDefinition, null, cancellationToken);
                    _messagingLogger.Trace($"{appName} resent {envelope.Id} as {resend.Id}");
                }

            }
            catch (Exception e)
            {
                _messagingLogger.Error(e, $"{appName} resend error");
            }
        }
    }
}