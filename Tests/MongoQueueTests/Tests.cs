using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoQueue;
using MongoQueue.Core;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.Logic;
using MongoQueue.Core.LogicAbstractions;
using MongoQueueTests.Common;
using NUnit.Framework;

namespace MongoQueueTests
{
    [TestFixture]
    public class Test : QueueTestsBase
    {
        protected override IMessagingDependencyRegistrator GetRegistrtor()
        {
            return new MessagingDependencyRegistrator();
        }

        protected override void DropCollection(string collectionName)
        {
            var mongoAgent = Resolver.Get<MongoAgent>();
            var db = mongoAgent.GetDb();
            db.DropCollection(collectionName);
        }

        [Test, RunInApplicationDomain]
        public async Task WhenMessageIsResent_SystemProcessesItOnce()
        {
            var subscriber = Builder.GetSubscriber();
            var configuration = (TestMessagingConfiguration)Resolver.Get<IMessagingConfiguration>();
            configuration.SetResends(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            subscriber.Subscribe<ResendHandler, TestMessage>();
            var publisher = Resolver.Get<IQueuePublisher>();
            var listener = Resolver.Get<QueueListener>();
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

            var subscriber = Resolver.Get<IQueueSubscriber>();
            subscriber.Subscribe<AnotherTestHandler, AnotherTestMessage>();
            var publisher = Resolver.Get<IQueuePublisher>();
            var listener = Resolver.Get<QueueListener>();
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
            var subscriber = Resolver.Get<IQueueSubscriber>();
            subscriber.Subscribe<TestHandler, TestMessage>();
            var publisher = Resolver.Get<IQueuePublisher>();
            var listener = Resolver.Get<QueueListener>();
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
            var subscriber = Resolver.Get<IQueueSubscriber>();
            subscriber.Subscribe<SlightlyDifferentTestHandler, SlightlyDifferentTestMessage>();
            var publisher = Resolver.Get<IQueuePublisher>();
            var listener = Resolver.Get<QueueListener>();
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
            var subscriber = Resolver.Get<IQueueSubscriber>();
            subscriber.Subscribe<SlightlyDifferentTestHandler, SlightlyDifferentTestMessage>();
            var publisher = Resolver.Get<IQueuePublisher>();
            var listener = Resolver.Get<QueueListener>();
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
            var subscriber = Resolver.Get<IQueueSubscriber>();
            subscriber.Subscribe<TimeConsumingHandler, TestMessage>();
            var publisher = Resolver.Get<IQueuePublisher>();
            var listener = Resolver.Get<QueueListener>();
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

        [Test, RunInApplicationDomain]
        public async Task WhenMessageSendWithoutSubscribers_ThenMessageShouldBeSentAfterSubscribing()
        {
            var publisher = Resolver.Get<IQueuePublisher>();
            var testMessage = CreateMessage();
            publisher.Publish(testMessage);
            var subscriber = Resolver.Get<IQueueSubscriber>();
            subscriber.Subscribe<TimeConsumingHandler, TestMessage>();
            var listener = Resolver.Get<QueueListener>();
            await listener.Start("test", CancellationToken.None);
            Throttle.Assert(
                () => ResultHolder.Contains(testMessage.Id) && ResultHolder.Count == 1,
                TimeSpan.FromSeconds(4),
                TimeSpan.FromSeconds(5)
                );
        }
    }
}