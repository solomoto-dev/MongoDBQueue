using System;
using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core;
using MongoQueue.Core.AgentAbstractions;

namespace MongoQueueTests.Common
{
    public class TransactionResendHandler : TransactionMessageHandlerBase<TestMessage>
    {

        public TransactionResendHandler(IMessageStatusManager messageStatusManager) : base(messageStatusManager)
        {
        }

        public override async Task HandleInTransaction(TestMessage message, bool resend, CancellationToken cancellationToken)
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