using System;
using System.Threading;
using System.Threading.Tasks;
using MongoQueueShared.Read;

namespace SecondReader
{
    public class AnotherDefaultHandler : MessageHandlerBase<AnotherDomainMessage>
    {
        public override async Task Handle(AnotherDomainMessage message, bool resend, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                Console.Write($"here comes dat boi {message.Oh} {message.Shit} {message.Waddap}");
            }
        }
    }
}