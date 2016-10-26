using System;
using System.Threading;
using System.Threading.Tasks;
using MongoQueueShared.Read;

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
                    //Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"processing resend {message.Id}");
                    //Console.ForegroundColor = ConsoleColor.White;
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