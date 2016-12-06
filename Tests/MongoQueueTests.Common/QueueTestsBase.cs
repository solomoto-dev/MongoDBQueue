using System;
using MongoQueue.Autofac;
using MongoQueue.Core;
using NUnit.Framework;

namespace MongoQueueTests.Common
{
    [TestFixture]
    public abstract class QueueTestsBase
    {
        protected IInstanceResolver Resolver { get; private set; }
        protected ConfiguredQueueBuilder Builder { get; private set; }
        [SetUp]
        public void Setup()
        {
            var autofacRegistrator = new AutofacRegistrator();
            var configurator = new QueueConfigurator(autofacRegistrator, GetRegistrtor());
            configurator
                .SetConfigurationProvider(TestMessagingConfiguration.Create())
                .SetTopicProvider<TestTopicNameProvider>()
                .RegisterHandler<TestHandler>()
                .RegisterHandler<SlightlyDifferentTestHandler>()
                .RegisterHandler<ResendHandler>()
                .RegisterHandler<TimeConsumingHandler>();
            Resolver = autofacRegistrator.CreateResolver();
            Builder = configurator.Build(Resolver);
            ClearDb();
        }

        [TearDown]
        public void TearDown()
        {
            ClearDb();
        }

        protected virtual void ClearDb()
        {
            DropCollection("test_Envelops");
            DropCollection("Subscriber");
            DropCollection("test2_Envelops");
            ResultHolder.Clear();
        }

        protected abstract IMessagingDependencyRegistrator GetRegistrtor();
        protected abstract void DropCollection(string collectionName);

        protected TestMessage CreateMessage()
        {
            return new TestMessage(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                new TestValueObject("123", "123123"));
        }
    }
}