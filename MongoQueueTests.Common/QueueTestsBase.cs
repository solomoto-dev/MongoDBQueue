using System;
using Autofac;
using MongoQueue.Autofac;
using MongoQueue.Core;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.IntegrationDefaults;
using MongoQueue.Core.LogicAbstractions;
using NUnit.Framework;

namespace MongoQueueTests.Common
{
    [TestFixture]
    public abstract class QueueTestsBase
    {
        [SetUp]
        public void Setup()
        {
            AutofacComposition.Compose(GetRegistrtor(), b =>
            {
                b.RegisterInstance(new DefaultMessagingConfiguration("mongodb://localhost:27017/test-queue", TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))).As<IMessagingConfiguration>();
                b.RegisterType<TestTopicNameProvider>().As<ITopicNameProvider>();
                b.RegisterType<TestHandler>();
                b.RegisterType<SlightlyDifferentTestHandler>();
                b.RegisterType<ResendHandler>();
                b.RegisterType<TimeConsumingHandler>();
            });
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