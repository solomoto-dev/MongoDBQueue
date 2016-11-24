using System;
using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core;
using MongoQueue.Core.AgentAbstractions;

namespace LegacyReader
{
    public class DefaultHandler : MessageHandlerBase<DomainMessage>
    {
        public DefaultHandler(IMessageStatusManager messageStatusManager) : base(messageStatusManager)
        {
        }

        public override async Task Handle(DomainMessage message, bool resend, CancellationToken cancellationToken)
        {
            await Task.Delay(1000, cancellationToken);
            if (!cancellationToken.IsCancellationRequested)
            {
                Console.Write($"handled {message.Id} {message.Name} {message.SomeDate}");
            }
        }
    }
}