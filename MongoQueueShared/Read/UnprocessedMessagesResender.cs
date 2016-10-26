using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoQueueShared.Common;

namespace MongoQueueShared.Read
{
    public class UnprocessedMessagesResender
    {
        private readonly MongoMessagingAgent _mongoMessagingAgent;
        private readonly IMessagingConfiguration _messagingConfiguration;

        public UnprocessedMessagesResender(
            MongoMessagingAgent mongoMessagingAgent, 
            IMessagingConfiguration messagingConfiguration
            )
        {
            _mongoMessagingAgent = mongoMessagingAgent;
            _messagingConfiguration = messagingConfiguration;
        }
        public async void Start(string appName, CancellationToken cancellationToken)
        {
            try
            {
                await RepeatEvery(() => Resend(appName, cancellationToken), _messagingConfiguration.ResendInterval);
            }
            catch (Exception)
            {
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
                    Builders<Envelope>.Filter.Lt(x => x.ProcessingStartedAt, DateTime.UtcNow - _messagingConfiguration.ResendTreshold),
                    Builders<Envelope>.Filter.Eq(x => x.ProcessedAt, DateTime.MinValue),
                    Builders<Envelope>.Filter.Ne(x => x.ProcessingStartedAt, DateTime.MinValue)
                    );
                var notProcessed =
                    await
                        (await collection.FindAsync(notProcessedFilter, cancellationToken: cancellationToken)).ToListAsync(
                            cancellationToken);
                foreach (var envelope in notProcessed)
                {
                    var resend = new Envelope(envelope.Topic, envelope.Payload, envelope.Id);
                    await collection.InsertOneAsync(resend, null, cancellationToken);
                    await collection.UpdateOneAsync(Builders<Envelope>.Filter.Eq("_id", envelope.Id), Builders<Envelope>.Update.Set(x => x.ProcessedAt, DateTime.UtcNow).Set(x => x.ResendId, resend.Id), null, cancellationToken);
                }

            }
            catch (Exception)
            {
                //Log:
                throw;
            }
        }
    }
}