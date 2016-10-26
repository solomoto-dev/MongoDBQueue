using System.Threading;
using System.Threading.Tasks;

namespace MongoQueue.Core.Read
{
    public abstract class MessageHandlerBase<TMessage> : MongoDependentHandlerBase<TMessage>, IHandler<TMessage>
    {
        public abstract override Task Handle(TMessage message, bool resend, CancellationToken cancellationToken);
    }
}