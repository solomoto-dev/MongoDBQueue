using System;
using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core;
using MongoQueue.Core.AgentAbstractions;

namespace MongoQueueTests.Common
{
    public class AnotherTestHandler : MessageHandlerBase<AnotherTestMessage>
    {
        public AnotherTestHandler(IMessageStatusManager messageStatusManager) : base(messageStatusManager)
        {
        }

        public override async Task Handle(AnotherTestMessage message, bool resend, CancellationToken cancellationToken)
        {
            Console.WriteLine($"{GetType().Name} {message.SomeDate} {message.UselessNumber}  {resend}");
            ResultHolder.Add(message.SomeDate.ToString(), message.UselessNumber.ToString());
        }

        
    }
}