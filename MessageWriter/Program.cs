using System;
using System.Threading.Tasks;
using MongoQueueShared;
using MongoQueueShared.Common;
using MongoQueueShared.Read;
using MongoQueueShared.Write;

namespace MessageWriter
{
    class Program
    {
        static void Main()
        {
            DoStuff().Wait();
        }

        static async Task DoStuff()
        {
            var messagingConfiguration = new DefaultMessagingConfiguration(null, TimeSpan.FromMilliseconds(300),
                TimeSpan.FromSeconds(1));
            var mongoHelper = new MongoMessagingAgent(messagingConfiguration);
            var publisher = new MongoQueuePublisher(new TopicNameProvider(), mongoHelper,new ConsoleMessagingLogger());
            while (true)
            {
                try
                {
                    var rnd = new Random(DateTime.Now.Millisecond);
                    var id = rnd.Next();
                    if (id % 2 == 0)
                    {
                        if (id % 4 == 0)
                        {
                            var message = new DomainMessage(id.ToString(), "exception");
                            await publisher.PublishAsync(message);
                        }
                        else
                        {
                            var message = new DomainMessage(id.ToString(), rnd.Next().ToString());
                            await publisher.PublishAsync(message);
                        }
                    }
                    else
                    {
                        var message = new AnotherDomainMessage(id.ToString(), rnd.Next().ToString(), "waddap indeed");
                        await publisher.PublishAsync(message);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    await Task.Delay(500);
                }

            }
        }
    }
}
