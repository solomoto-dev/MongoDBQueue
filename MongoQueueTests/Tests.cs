using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core.Common;
using MongoQueue.Core.Read;
using MongoQueue.Core.Write;
using NUnit.Framework;

namespace MongoQueueTests
{
    [TestFixture]
    public class Test
    {
        private MongoQueuePublisher _publisher;
        private CancellationTokenSource _cancellationTokenSource;

        [SetUp]
        public void Setup()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var messagingConfiguration = new DefaultMessagingConfiguration(null, TimeSpan.FromMilliseconds(300), TimeSpan.FromSeconds(1));
            var mongoHelper = new MongoMessagingAgent(messagingConfiguration);
            _publisher = new MongoQueuePublisher(new TopicNameProvider(), mongoHelper, new ConsoleMessagingLogger());
        }

        [TearDown]
        public void TearDown()
        {
            var messagingConfiguration = new DefaultMessagingConfiguration(null, TimeSpan.FromMilliseconds(300), TimeSpan.FromSeconds(1));
            var mongoHelper = new MongoMessagingAgent(messagingConfiguration);
            var db = mongoHelper.GetDb();
            db.DropCollection("test_Envelops");
            db.DropCollection("Subscriber");
            db.DropCollection("test2_Envelops");
            ResultHolder.Clear();
            _cancellationTokenSource.Cancel();

        }
        private MongoMessageListener CreateListener(bool resend = false)
        {
            var messagingConfiguration = new DefaultMessagingConfiguration(null, TimeSpan.FromMilliseconds(300), TimeSpan.FromSeconds(1));
            var topicNameProvider = new TopicNameProvider();
            var messageTypesCache = new MessageTypesCache(topicNameProvider);
            var messageHandlersCache = new MessageHandlersCache(topicNameProvider);
            var mongoHelper = new MongoMessagingAgent(messagingConfiguration);
            if (resend)
            {
                messageTypesCache.Register<TestMessage>();
                messageHandlersCache.Register<ResendHandler, TestMessage>();
            }
            else
            {
                messageTypesCache.Register<TestMessage>();
                messageTypesCache.Register<AnotherTestMessage>();
                messageHandlersCache.Register<TestHandler, TestMessage>();
                messageHandlersCache.Register<AnotherTestHandler, AnotherTestMessage>();
            }
            var consoleMessagingLogger = new ConsoleMessagingLogger();
            var messageProcessor = new MessageProcessor(messageHandlersCache, messageTypesCache, new ActivatorMessageHandlerFactory(), consoleMessagingLogger);
            var unprocessedMessagesResender = new UnprocessedMessagesResender(new MongoMessagingAgent(messagingConfiguration), messagingConfiguration, consoleMessagingLogger);
            return new MongoMessageListener(messageTypesCache, mongoHelper, consoleMessagingLogger, messageProcessor, unprocessedMessagesResender);
        }

        [Test, RunInApplicationDomain]
        public async Task WhenMessageIsPosted_ApplicationThatIsNotSubscribedToItDoesntGetIt()
        {
            var messagingConfiguration = new DefaultMessagingConfiguration(null, TimeSpan.FromMilliseconds(300), TimeSpan.FromSeconds(1));
            var topicNameProvider = new TopicNameProvider();
            var messageTypesCache = new MessageTypesCache(topicNameProvider);
            var messageHandlersCache = new MessageHandlersCache(topicNameProvider);
            var mongoHelper = new MongoMessagingAgent(messagingConfiguration);
            messageTypesCache.Register<AnotherTestMessage>();
            messageHandlersCache.Register<AnotherTestHandler, AnotherTestMessage>();
            var consoleMessagingLogger = new ConsoleMessagingLogger();
            var messageProcessor = new MessageProcessor(messageHandlersCache, messageTypesCache, new ActivatorMessageHandlerFactory(), consoleMessagingLogger);
            var unprocessedMessagesResender = new UnprocessedMessagesResender(new MongoMessagingAgent(messagingConfiguration), messagingConfiguration, consoleMessagingLogger);
            var listener = new MongoMessageListener(messageTypesCache, mongoHelper, consoleMessagingLogger, messageProcessor, unprocessedMessagesResender);
            await listener.Start("test", _cancellationTokenSource.Token);
            await _publisher.PublishAsync(new TestMessage("id", "name", new TestValueObject("id", "name")));
            Throttle.Assert(() => Assert.True(ResultHolder.Count == 0), TimeSpan.FromSeconds(1));
        }

        [Test, RunInApplicationDomain]
        public async Task WhenMessageIsResent_SystemProcessesItOnce()
        {
            var listener = CreateListener(true);
            await listener.Start("test", _cancellationTokenSource.Token);
            await _publisher.PublishAsync(new TestMessage("id", "name", new TestValueObject("id", "name")));
            var timeout = TimeSpan.FromMilliseconds(5000);
            if (Debugger.IsAttached)
            {
                timeout = TimeSpan.FromMinutes(1);
            }
            Throttle.Assert(() => Assert.True(ResultHolder.Contains("idresend")), TimeSpan.FromSeconds(1), timeout);
            Throttle.Assert(() => Assert.True(ResultHolder.Count == 1));
        }

        [Test, RunInApplicationDomain]
        public async Task WhenMessageIsPosted_TargetApplicationProcessesItOnce()
        {
            var listener = CreateListener();
            await listener.Start("test", _cancellationTokenSource.Token);
            await _publisher.PublishAsync(new TestMessage("id", "name", new TestValueObject("id", "name")));
            Throttle.Assert(() => Assert.True(ResultHolder.Contains("id")), TimeSpan.FromMilliseconds(2000));
            Throttle.Assert(() => Assert.True(ResultHolder.Count == 1));
        }

        [Test, RunInApplicationDomain]
        public async Task WhenMessageWithSlightlyDifferentStructureButWithSameTopicIsPosted_ItCanbeProcessed()
        {
            var messagingConfiguration = new DefaultMessagingConfiguration(null, TimeSpan.FromMilliseconds(300), TimeSpan.FromSeconds(1));
            var topicNameProvider = new TestTopicNameProvider();
            var messageTypesCache = new MessageTypesCache(topicNameProvider);
            var messageHandlersCache = new MessageHandlersCache(topicNameProvider);
            var mongoHelper = new MongoMessagingAgent(messagingConfiguration);
            messageTypesCache.Register<SlightlyDifferentTestMessage>();
            messageHandlersCache.Register<SlightlyDifferentTestHandler, SlightlyDifferentTestMessage>();
            var consoleMessagingLogger = new ConsoleMessagingLogger();
            var messageProcessor = new MessageProcessor(messageHandlersCache, messageTypesCache, new ActivatorMessageHandlerFactory(), consoleMessagingLogger);
            var unprocessedMessagesResender = new UnprocessedMessagesResender(new MongoMessagingAgent(messagingConfiguration), messagingConfiguration, consoleMessagingLogger);
            var listener = new MongoMessageListener(messageTypesCache, mongoHelper, consoleMessagingLogger, messageProcessor, unprocessedMessagesResender);
            await listener.Start("test", _cancellationTokenSource.Token);
            await _publisher.PublishAsync(new TestMessage("id", "name", new TestValueObject("id", "name")));
            Throttle.Assert(() => Assert.True(ResultHolder.Contains("id")), TimeSpan.FromMilliseconds(2000));
        }
    }
}
