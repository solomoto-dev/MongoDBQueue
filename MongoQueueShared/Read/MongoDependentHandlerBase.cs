using System;
using System.Threading;
using System.Threading.Tasks;
using MongoQueueShared.Common;
using Builder = MongoDB.Driver.Builders<MongoQueueShared.Common.Envelope>;

namespace MongoQueueShared.Read
{
    public abstract class MongoDependentHandlerBase<TMessage> : IHandler
    {
        private readonly MongoMessagingAgent _mongoMessagingAgent;

        protected MongoDependentHandlerBase(MongoMessagingAgent mongoMessagingAgent = null)
        {
            var messagingConfiguration = new DefaultMessagingConfiguration(null, TimeSpan.FromMilliseconds(300), TimeSpan.FromSeconds(1));
            _mongoMessagingAgent = mongoMessagingAgent ??  new MongoMessagingAgent(messagingConfiguration);
        }

        public abstract Task Handle(TMessage message, bool resend, CancellationToken cancellationToken);

        public async Task Handle(string appName, string messageId, object message, bool resend, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                if (await SetProcessingStart(appName, messageId, cancellationToken))
                {
                    await Handle((TMessage)message, resend, cancellationToken);
                    await SetProcessedAt(appName, messageId, cancellationToken);
                }
            }
        }

        private async Task<bool> SetProcessingStart(string appName, string messageId, CancellationToken cancellationToken)
        {
            var collection = _mongoMessagingAgent.GetEnvelops(appName);
            var filter = Builder.Filter.And(Builder.Filter.Eq("_id", messageId), Builder.Filter.Eq(x => x.ProcessedAt, DateTime.MinValue));
            var found = await collection.FindOneAndUpdateAsync<Envelope>(filter,
                Builder.Update.Set(x => x.ProcessingStartedAt, DateTime.UtcNow), cancellationToken: cancellationToken);
            return found != null;
        }

        private async Task SetProcessedAt(string appName, string messageId, CancellationToken cancellationToken)
        {
            var collection = _mongoMessagingAgent.GetEnvelops(appName);
            await collection.FindOneAndUpdateAsync<Envelope>(Builder.Filter.Eq("_id", messageId),
                Builder.Update.Set(x => x.ProcessedAt, DateTime.UtcNow), cancellationToken: cancellationToken);
        }
    }
}