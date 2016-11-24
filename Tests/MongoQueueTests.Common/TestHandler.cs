using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core;
using MongoQueue.Core.AgentAbstractions;

namespace MongoQueueTests.Common
{
    public class TimeConsumingHandler : MessageHandlerBase<TestMessage>
    {
        public TimeConsumingHandler(IMessageStatusManager messageStatusManager) : base(messageStatusManager)
        {
        }

        public override async Task Handle(TestMessage message, bool resend, CancellationToken cancellationToken)
        {
            await Task.Delay(3000,cancellationToken);
            ResultHolder.Add(message.Id, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
        }
    }

    public class TestHandler : MessageHandlerBase<TestMessage>
    {
        public TestHandler(IMessageStatusManager messageStatusManager) : base(messageStatusManager)
        {
        }

        public override async Task Handle(TestMessage message, bool resend, CancellationToken cancellationToken)
        {
            Console.WriteLine($"{GetType().Name} {message.Id} {message.Name}  {resend}");
            ResultHolder.Add(message.Id, message.Name);
        }
    }
}