using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core.AgentAbstractions;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core
{
    public abstract class MessageHandlerBase<TMessage> : MongoDependentHandlerBase<TMessage>, IHandler<TMessage>
    {
        protected MessageHandlerBase(IMessageStatusManager messageStatusManager) : base(messageStatusManager)
        {
        }
        public abstract override Task Handle(TMessage message, bool resend, CancellationToken cancellationToken);
    }
}