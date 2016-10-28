using System;
using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core;
using MongoQueue.Core.AgentAbstractions;

namespace MongoQueueTests.Common
{
    public class SlightlyDifferentTestHandler : MessageHandlerBase<SlightlyDifferentTestMessage>
    {
        public SlightlyDifferentTestHandler(IMessageStatusManager messageStatusManager) : base(messageStatusManager)
        {
        }

        public override async Task Handle(SlightlyDifferentTestMessage message, bool resend,
            CancellationToken cancellationToken)
        {
            Console.WriteLine(
                $"{GetType().Name} {message.Id} {message.TestValueObject.Id} {message.TestValueObject.SomeDate} {resend}");
            ResultHolder.Add(message.Id, message.TestValueObject.Id);
        }

    }
}