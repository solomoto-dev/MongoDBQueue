using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            var containerBuilder = new ContainerBuilder();
            var autofacRegistrator = new AutofacRegistrator(containerBuilder);

            var serviceProvider = new ServiceCollection
            {
                new ServiceDescriptor(
                    typeof(IContainer),
                    provider => autofacRegistrator.Container,
                    ServiceLifetime.Singleton)
            };

            containerBuilder.Populate(serviceProvider);
            var configurator = new QueueConfigurator(autofacRegistrator, GetRegistrator());
            configurator
                .SetConfigurationProvider(TestMessagingConfiguration.Create())
                .SetTopicProvider<TestTopicNameProvider>()
                .RegisterHandler<TestHandler>()
                .RegisterHandler<SlightlyDifferentTestHandler>()
                .RegisterHandler<AlwaysErrorHandler>()
                .RegisterHandler<ResendHandler>()
                .RegisterHandler<TransactionResendHandler>()
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
            DropCollection("DeadLetter");
            ResultHolder.Clear();
        }

        protected abstract IMessagingDependencyRegistrator GetRegistrator();
        protected abstract void DropCollection(string collectionName);

        protected TestMessage CreateMessage()
        {
            return new TestMessage(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                new TestValueObject("123", "123123"));
        }
    }
}