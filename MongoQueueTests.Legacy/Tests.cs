using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MongoQueue.Autofac;
using MongoQueue.Core;
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
    public class Tests : QueueTestsBase
    {
        protected override IMessagingDependencyRegistrator GetRegistrtor()
        {
            return new LegacyMessagingDependencyRegistrator();
        }

        protected override void DropCollection(string collectionName)
        {
            var mongoAgent = AutofacComposition.Container.Resolve<LegacyMongoAgent>();
            var db = mongoAgent.GetDb();
            db.DropCollection(collectionName);
        }


        [Test, RunInApplicationDomain]
        public async Task WhenMessageIsResent_SystemProcessesItOnce()
        {
            var subscriber = AutofacComposition.Container.Resolve<IQueueSubscriber>();
            var configuration = (DefaultMessagingConfiguration)AutofacComposition.Container.Resolve<IMessagingConfiguration>();
            configuration.ResendInterval = TimeSpan.FromSeconds(1);
            configuration.ResendTreshold = TimeSpan.FromSeconds(1);
            subscriber.Subscribe<ResendHandler, TestMessage>();
            var publisher = AutofacComposition.Container.Resolve<IQueuePublisher>();
            var listener = AutofacComposition.Container.Resolve<QueueListener>();
            await listener.Start("test", CancellationToken.None);
            var testMessage = CreateMessage();
            publisher.Publish(testMessage);
            Throttle.Assert(
                () => ResultHolder.Contains(testMessage.Id + "resend") && ResultHolder.Count == 1,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(6)
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
                TimeSpan.FromSeconds(6)
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
                TimeSpan.FromSeconds(6)
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
                TimeSpan.FromSeconds(6)
                );
        }

        [Test, RunInApplicationDomain]
        [TestCase(3)]
        public async Task WhenNMessagesAreSent_NMessagesHandled(int messagesCount)
        {
            var subscriber = AutofacComposition.Container.Resolve<IQueueSubscriber>();
            subscriber.Subscribe<SlightlyDifferentTestHandler, SlightlyDifferentTestMessage>();
            var publisher = AutofacComposition.Container.Resolve<IQueuePublisher>();
            var listener = AutofacComposition.Container.Resolve<QueueListener>();
            await listener.Start("test", CancellationToken.None);
            var testMessages = Enumerable.Range(0, messagesCount).Select(x => CreateMessage()).ToArray();
            foreach (var testMessage in testMessages)
            {
                publisher.Publish(testMessage);
            }
            Throttle.Assert(
                () => ResultHolder.Contains(testMessages.Select(x => x.Id).ToArray()) && ResultHolder.Count == testMessages.Length,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(6)
                );
        }


        [Test, RunInApplicationDomain]
        [TestCase(5)]
        public async Task WhenNMessagesAreSent_TheyAreHandledSimultaneously(int messagesCount)
        {
            var subscriber = AutofacComposition.Container.Resolve<IQueueSubscriber>();
            subscriber.Subscribe<TimeConsumingHandler, TestMessage>();
            var publisher = AutofacComposition.Container.Resolve<IQueuePublisher>();
            var listener = AutofacComposition.Container.Resolve<QueueListener>();
            await listener.Start("test", CancellationToken.None);
            var testMessages = Enumerable.Range(0, messagesCount).Select(x => CreateMessage()).ToArray();
            foreach (var testMessage in testMessages)
            {
                publisher.Publish(testMessage);
            }
            Throttle.Assert(
                () => ResultHolder.Contains(testMessages.Select(x => x.Id).ToArray()) && ResultHolder.Count == testMessages.Length,
                TimeSpan.FromSeconds(4),
                TimeSpan.FromSeconds(5)
                );
        }
    }
}