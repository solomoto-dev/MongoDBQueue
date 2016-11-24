using System;
using System.Threading.Tasks;
using Autofac;
using MongoQueue.Autofac;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.IntegrationDefaults;
using MongoQueue.Core.LogicAbstractions;
using MongoQueue.Legacy;

namespace LegacyWriter
{
    class Program
    {
        static void Main()
        {
            DoStuff().Wait();
        }

        static async Task DoStuff()
        {
            AutofacComposition.Compose(new LegacyMessagingDependencyRegistrator(), b =>
            {
                b.RegisterInstance(new DefaultMessagingConfiguration("mongodb://localhost:27017","dev-queue", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30))).As<IMessagingConfiguration>();
            });
            var publisher = AutofacComposition.Container.Resolve<IQueuePublisher>();
            while (true)
            {
                try
                {
                    var rnd = new Random(DateTime.Now.Millisecond);
                    var id = rnd.Next();
                    var guid = Guid.NewGuid().ToString();
                    if (id % 2 == 0)
                    {
                        if (id % 4 == 0)
                        {
                            var message = new DomainMessage(guid, "exception");
                            await publisher.PublishAsync(message);
                        }
                        else
                        {
                            var message = new DomainMessage(guid, rnd.Next().ToString());
                            await publisher.PublishAsync(message);
                        }
                    }
                    else
                    {
                        var message = new AnotherDomainMessage(guid, rnd.Next().ToString(), "waddap indeed");
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
