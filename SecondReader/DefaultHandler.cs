using System;
using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core.Read;

namespace SecondReader
{
    public class DefaultHandler : MessageHandlerBase<DomainMessage>
    {
        public override async Task Handle(DomainMessage message, bool resend, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                if (resend)
                {
                    Console.Write($"processing resend {message.Id}");
                    return;
                }

                if (message.Name == "exception")
                {
                    throw new InvalidOperationException("omgwtf");
                }
                else
                {
                    Console.Write($"handled {message.Id} {message.Name} {message.UselessField}");
                }
            }
        }
    }
}