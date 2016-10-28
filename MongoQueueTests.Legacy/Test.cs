using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MongoQueue.Autofac;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.IntegrationDefaults;
using MongoQueue.Core.Logic;
using MongoQueue.Core.LogicAbstractions;
using MongoQueue.Legacy;
using MongoQueueTests.Common;
using NUnit.Framework;

namespace MongoQueueTests.Legacy
{
    [TestFixture]
    public class Test
    {
        [SetUp]
        public void Setup()
        {
            AutofacComposition.Compose(new LegacyMessagingDependencyRegistrator(), b =>
            {
                b.RegisterInstance(new DefaultMessagingConfiguration("mongodb://localhost:27017/test-queue", TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))).As<IMessagingConfiguration>();
                b.RegisterType<TestTopicNameProvider>().As<ITopicNameProvider>();
                b.RegisterType<TestHandler>();
                b.RegisterType<SlightlyDifferentTestHandler>();
                b.RegisterType<ResendHandler>();
            });
            ClearDb();
        }

        [TearDown]
        public void TearDown()
        {
            ClearDb();
        }

        private static void ClearDb()
        {
            var mongoAgent = AutofacComposition.Container.Resolve<LegacyMongoAgent>();
            var db = mongoAgent.GetDb();
            db.DropCollection("test_Envelops");
            db.DropCollection("Subscriber");
            db.DropCollection("test2_Envelops");
            ResultHolder.Clear();
        }


        [Test, RunInApplicationDomain]
        public async Task WhenMessageIsResent_SystemProcessesItOnce()
        {
            var subscriber = AutofacComposition.Container.Resolve<IQueueSubscriber>();
            subscriber.Subscribe<ResendHandler, TestMessage>();
            var publisher = AutofacComposition.Container.Resolve<IQueuePublisher>();
            var listener = AutofacComposition.Container.Resolve<QueueListener>();
            await listener.Start("test", CancellationToken.None);
            var testMessage = CreateMessage();
            publisher.Publish(testMessage);
            Throttle.Assert(
                () => ResultHolder.Contains(testMessage.Id + "resend") && ResultHolder.Count == 1,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(5)
                );
        }

        [Test, RunInApplicationDomain]
        public async Task WhenMessageIsPosted_ApplicationThatIsNotSubscribedToItDoesntGetIt()
        {

            var subscriber = AutofacComposition.Container.Resolve<IQueueSubscriber>();
            subscriber.Subscribe<AnotherTestHandler, AnotherTestMessage>();
            var publisher = AutofacComposition.Container.Resolve<IQueuePublisher>();
            var listener = AutofacComposition.Container.Resolve<QueueListener>();
            await listener.Start("test", CancellationToken.None);
            var testMessage = CreateMessage();
            publisher.Publish(testMessage);
            Throttle.Assert(
                () => ResultHolder.Count == 0,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(5)
                );
        }

        [Test, RunInApplicationDomain]
        public async Task WhenMessageIsPosted_TargetApplicationProcessesItOnce()
        {
            var subscriber = AutofacComposition.Container.Resolve<IQueueSubscriber>();
            subscriber.Subscribe<TestHandler, TestMessage>();
            var publisher = AutofacComposition.Container.Resolve<IQueuePublisher>();
            var listener = AutofacComposition.Container.Resolve<QueueListener>();
            await listener.Start("test", CancellationToken.None);
            var testMessage = CreateMessage();
            publisher.Publish(testMessage);
            Throttle.Assert(
                () => ResultHolder.Contains(testMessage.Id) && ResultHolder.Count == 1,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(5)
                );
        }

        [Test, RunInApplicationDomain]
        public async Task WhenMessageWithSlightlyDifferentStructureButWithSameTopicIsPosted_ItCanbeProcessed()
        {
            var subscriber = AutofacComposition.Container.Resolve<IQueueSubscriber>();
            subscriber.Subscribe<SlightlyDifferentTestHandler, SlightlyDifferentTestMessage>();
            var publisher = AutofacComposition.Container.Resolve<IQueuePublisher>();
            var listener = AutofacComposition.Container.Resolve<QueueListener>();
            await listener.Start("test", CancellationToken.None);
            var testMessage = CreateMessage();
            publisher.Publish(testMessage);
            Throttle.Assert(
                () => ResultHolder.Contains(testMessage.Id) && ResultHolder.Count == 1,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(5)
                );
        }

        private TestMessage CreateMessage()
        {
            return new TestMessage(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                new TestValueObject("123", "123123"));
        }
    }
}