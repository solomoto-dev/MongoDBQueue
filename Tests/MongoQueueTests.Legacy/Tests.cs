using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core;
using MongoQueue.Core.Exceptions;
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
        protected override IMessagingDependencyRegistrator GetRegistrator()
        {
            return new LegacyMessagingDependencyRegistrator();
        }

        protected override void DropCollection(string collectionName)
        {
            var mongoAgent = Resolver.Get<LegacyMongoAgent>();
            var db = mongoAgent.Db;
            db.DropCollection(collectionName);
        }

        #region sync

        [Test, RunInApplicationDomain]
        public async Task WhenMessageIsResent_SystemProcessesItOnce()
        {
            var subscriber = Resolver.Get<IQueueSubscriber>();
            var configuration = (TestMessagingConfiguration)Resolver.Get<IMessagingConfiguration>();
            configuration.SetResends(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            subscriber.Subscribe<ResendHandler, TestMessage>();
            var publisher = Builder.GetPublisher();
            var listener = Builder.GetListener();
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
        public async Task WhenMessageIsResentInTransactionHandler_SystemProcessesItOnce()
        {
            var subscriber = Resolver.Get<IQueueSubscriber>();
            var configuration = (TestMessagingConfiguration)Resolver.Get<IMessagingConfiguration>();
            configuration.SetResends(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            subscriber.Subscribe<TransactionResendHandler, TestMessage>();
            var publisher = Builder.GetPublisher();
            var listener = Builder.GetListener();
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
        public async Task WhenMessageResentMoreThan10Times_ResendingStops()
        {
            var configuration = (TestMessagingConfiguration)Resolver.Get<IMessagingConfiguration>();
            configuration.SetResends(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5));
            var subscriber = Resolver.Get<IQueueSubscriber>();
            subscriber.Subscribe<AlwaysErrorHandler, TestMessage>();
            var publisher = Resolver.Get<IQueuePublisher>();
            var listener = Resolver.Get<QueueListener>();
            await listener.Start("test", CancellationToken.None);
            var testMessage = CreateMessage();
            publisher.Publish(testMessage);
            Throttle.Assert(
                () => ResultHolder.Count == 11,
                TimeSpan.FromSeconds(15),
                TimeSpan.FromSeconds(20)
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

        [Test, RunInApplicationDomain]
        public void WhenPublishToNoMongoEndpoint_ThenShouldExplode()
        {
            Setup(new DefaultMessagingConfiguration("mongodb://holocaust:27017", "dev-queue", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), CursorType.Polling, 10), false);
            var publisher = Resolver.Get<IQueuePublisher>();
            var testMessage = CreateMessage();
            Assert.Throws<QueueConfigurationException>(() => publisher.Publish(testMessage));
        }

        #endregion

        #region async

        [Test, RunInApplicationDomain]
        public async Task WhenMessageIsResentAsync_SystemProcessesItOnce()
        {
            var subscriber = Resolver.Get<IQueueSubscriber>();
            var configuration = (TestMessagingConfiguration)Resolver.Get<IMessagingConfiguration>();
            configuration.SetResends(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            subscriber.Subscribe<ResendHandler, TestMessage>();
            var publisher = Builder.GetPublisher();
            var listener = Builder.GetListener();
            await listener.Start("test", CancellationToken.None);
            var testMessage = CreateMessage();
            await publisher.PublishAsync(testMessage);
            Throttle.Assert(
                () => ResultHolder.Contains(testMessage.Id + "resend") && ResultHolder.Count == 1,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(6)
            );
        }

        [Test, RunInApplicationDomain]
        public async Task WhenMessageIsResentAsyncInTransactionHandler_SystemProcessesItOnce()
        {
            var subscriber = Resolver.Get<IQueueSubscriber>();
            var configuration = (TestMessagingConfiguration)Resolver.Get<IMessagingConfiguration>();
            configuration.SetResends(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            subscriber.Subscribe<TransactionResendHandler, TestMessage>();
            var publisher = Builder.GetPublisher();
            var listener = Builder.GetListener();
            await listener.Start("test", CancellationToken.None);
            var testMessage = CreateMessage();
            await publisher.PublishAsync(testMessage);
            Throttle.Assert(
                () => ResultHolder.Contains(testMessage.Id + "resend") && ResultHolder.Count == 1,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(6)
            );
        }

        [Test, RunInApplicationDomain]
        public async Task WhenMessageIsPostedAsync_ApplicationThatIsNotSubscribedToItDoesntGetIt()
        {

            var subscriber = Resolver.Get<IQueueSubscriber>();
            subscriber.Subscribe<AnotherTestHandler, AnotherTestMessage>();
            var publisher = Resolver.Get<IQueuePublisher>();
            var listener = Resolver.Get<QueueListener>();
            await listener.Start("test", CancellationToken.None);
            var testMessage = CreateMessage();
            await publisher.PublishAsync(testMessage);
            Throttle.Assert(
                () => ResultHolder.Count == 0,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(6)
            );
        }

        [Test, RunInApplicationDomain]
        public async Task WhenMessageIsPostedAsync_TargetApplicationProcessesItOnce()
        {
            var subscriber = Resolver.Get<IQueueSubscriber>();
            subscriber.Subscribe<TestHandler, TestMessage>();
            var publisher = Resolver.Get<IQueuePublisher>();
            var listener = Resolver.Get<QueueListener>();
            await listener.Start("test", CancellationToken.None);
            var testMessage = CreateMessage();
            await publisher.PublishAsync(testMessage);
            Throttle.Assert(
                () => ResultHolder.Contains(testMessage.Id) && ResultHolder.Count == 1,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(6)
            );
        }

        [Test, RunInApplicationDomain]
        public async Task WhenMessageWithSlightlyDifferentStructureButWithSameTopicIsPostedAsync_ItCanbeProcessed()
        {
            var subscriber = Resolver.Get<IQueueSubscriber>();
            subscriber.Subscribe<SlightlyDifferentTestHandler, SlightlyDifferentTestMessage>();
            var publisher = Resolver.Get<IQueuePublisher>();
            var listener = Resolver.Get<QueueListener>();
            await listener.Start("test", CancellationToken.None);
            var testMessage = CreateMessage();
            await publisher.PublishAsync(testMessage);
            Throttle.Assert(
                () => ResultHolder.Contains(testMessage.Id) && ResultHolder.Count == 1,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(6)
            );
        }


        [Test, RunInApplicationDomain]
        public async Task WhenMessageResentAsyncMoreThan10Times_ResendingStops()
        {
            var configuration = (TestMessagingConfiguration)Resolver.Get<IMessagingConfiguration>();
            configuration.SetResends(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5));
            var subscriber = Resolver.Get<IQueueSubscriber>();
            subscriber.Subscribe<AlwaysErrorHandler, TestMessage>();
            var publisher = Resolver.Get<IQueuePublisher>();
            var listener = Resolver.Get<QueueListener>();
            await listener.Start("test", CancellationToken.None);
            var testMessage = CreateMessage();
            await publisher.PublishAsync(testMessage);
            Throttle.Assert(
                () => ResultHolder.Count == 11,
                TimeSpan.FromSeconds(15),
                TimeSpan.FromSeconds(20)
            );
        }

        [Test, RunInApplicationDomain]
        [TestCase(3)]
        public async Task WhenNMessagesAreSentAsync_NMessagesHandled(int messagesCount)
        {
            var subscriber = Resolver.Get<IQueueSubscriber>();
            subscriber.Subscribe<SlightlyDifferentTestHandler, SlightlyDifferentTestMessage>();
            var publisher = Resolver.Get<IQueuePublisher>();
            var listener = Resolver.Get<QueueListener>();
            await listener.Start("test", CancellationToken.None);
            var testMessages = Enumerable.Range(0, messagesCount).Select(x => CreateMessage()).ToArray();
            await Task.WhenAll(testMessages.Select(testMessage => publisher.PublishAsync(testMessage)));
            Throttle.Assert(
                () => ResultHolder.Contains(testMessages.Select(x => x.Id).ToArray()) && ResultHolder.Count == testMessages.Length,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(6)
            );
        }

        [Test, RunInApplicationDomain]
        [TestCase(5)]
        public async Task WhenNMessagesAreSentAsync_TheyAreHandledSimultaneously(int messagesCount)
        {
            var subscriber = Resolver.Get<IQueueSubscriber>();
            subscriber.Subscribe<TimeConsumingHandler, TestMessage>();
            var publisher = Resolver.Get<IQueuePublisher>();
            var listener = Resolver.Get<QueueListener>();
            await listener.Start("test", CancellationToken.None);
            var testMessages = Enumerable.Range(0, messagesCount).Select(x => CreateMessage()).ToArray();
            await Task.WhenAll(testMessages.Select(testMessage => publisher.PublishAsync(testMessage)));
            Throttle.Assert(
                () => ResultHolder.Contains(testMessages.Select(x => x.Id).ToArray()) && ResultHolder.Count == testMessages.Length,
                TimeSpan.FromSeconds(4),
                TimeSpan.FromSeconds(5)
            );
        }

        [Test, RunInApplicationDomain]
        public async Task WhenMessageSendAsyncWithoutSubscribers_ThenMessageShouldBeSentAfterSubscribing()
        {
            var publisher = Resolver.Get<IQueuePublisher>();
            var testMessage = CreateMessage();
            await publisher.PublishAsync(testMessage);
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

        [Test, RunInApplicationDomain]
        public void WhenPublishAsyncToNoMongoEndpoint_ThenShouldExplode()
        {
            Setup(new DefaultMessagingConfiguration("mongodb://holocaust:27017", "dev-queue", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), CursorType.Polling, 10), false);
            var publisher = Resolver.Get<IQueuePublisher>();
            var testMessage = CreateMessage();
            Assert.ThrowsAsync<QueueConfigurationException>(() => publisher.PublishAsync(testMessage));
        }

        #endregion

        [Test, RunInApplicationDomain]
        public void WhenSubscribeToNoMongoEndpoint_ThenShouldExplode()
        {
            Setup(new DefaultMessagingConfiguration("mongodb://holocaust:27017", "dev-queue", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), CursorType.Polling, 10), false);
            var subscriber = Resolver.Get<IQueueSubscriber>();
            subscriber.Subscribe<TimeConsumingHandler, TestMessage>();
            var listener = Resolver.Get<QueueListener>();
            Assert.ThrowsAsync<QueueConfigurationException>(() => listener.Start("test", CancellationToken.None));
        }
    }
}