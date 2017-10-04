using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core.AgentAbstractions;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core
{
    public abstract class MongoDependentHandlerBase<TMessage> : IHandler
    {
        private readonly IMessageStatusManager _messageStatusManager;

        protected MongoDependentHandlerBase(IMessageStatusManager messageStatusManager)
        {
            _messageStatusManager = messageStatusManager;
        }

        public abstract Task Handle(TMessage message, bool resend, CancellationToken cancellationToken);

        public async Task Handle(string route, string messageId, object message, bool resend, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                if (await _messageStatusManager.TrySetProcessingStartedAt(route, messageId, cancellationToken))
                {
                    await Handle((TMessage)message, resend, cancellationToken);
                    await _messageStatusManager.SetProcessedAt(route, messageId, cancellationToken);
                }
            }
        }
    }
}