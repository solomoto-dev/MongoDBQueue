using System;
using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core;
using MongoQueue.Core.AgentAbstractions;

namespace MongoQueueTests.Common
{
    public class AlwaysErrorHandler : MessageHandlerBase<TestMessage>
    {
        public AlwaysErrorHandler(IMessageStatusManager messageStatusManager) : base(messageStatusManager)
        {
        }

        public override async Task Handle(TestMessage message, bool resend, CancellationToken cancellationToken)
        {
            ResultHolder.Add(message.Id + "resend" + Guid.NewGuid(), message.Name);
            throw new InvalidOperationException();
        }
    }
}