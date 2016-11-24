using System;
using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core;
using MongoQueue.Core.AgentAbstractions;

namespace MongoQueueTests.Common
{
    public class ResendHandler : MessageHandlerBase<TestMessage>
    {

        public ResendHandler(IMessageStatusManager messageStatusManager) : base(messageStatusManager)
        {
        }

        public override async Task Handle(TestMessage message, bool resend, CancellationToken cancellationToken)
        {
            Console.WriteLine($"{GetType().Name} {message.Id} {message.Name}  {resend}");
            if (resend)
            {
                ResultHolder.Add(message.Id + "resend", message.Name);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}