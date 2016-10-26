using System;
using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core.Read;

namespace MongoQueueReader
{
    public class DefaultHandler : MessageHandlerBase<DomainMessage>
    {
        public override async Task Handle(DomainMessage message, bool resend, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                Console.Write($"handled {message.Id} {message.Name} {message.SomeDate}");
            }
        }
    }
}