using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MongoQueue.Core;
using MongoQueue.Core.Exceptions;
using MongoQueue.Core.IntegrationAbstractions;
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
            Setup(null);
        }

        public void Setup(IMessagingConfiguration config, bool deleteDb = true)
        {
            if (config == null) config = TestMessagingConfiguration.Create();
            var containerBuilder = new ContainerBuilder();
            var serviceProvider = new ServiceCollection();
            var configurator = new QueueConfigurator(serviceProvider, GetRegistrator());
            configurator
                .SetConfigurationProvider(config)
                .SetTopicProvider<TestTopicNameProvider>()
                .RegisterHandler<TestHandler>()
                .RegisterHandler<SlightlyDifferentTestHandler>()
                .RegisterHandler<AlwaysErrorHandler>()
                .RegisterHandler<ResendHandler>()
                .RegisterHandler<TransactionResendHandler>()
                .RegisterHandler<TimeConsumingHandler>();
            serviceProvider.AddSingleton(configurator);
            serviceProvider.AddSingleton<IInstanceResolver, ServiceProviderResolver>();
            containerBuilder.Populate(serviceProvider);
            var container = containerBuilder.Build();
            Resolver = container.Resolve<IInstanceResolver>();
            Builder = configurator.Build(Resolver);
            if(deleteDb) ClearDb();            
        }        

        [TearDown]
        public void TearDown()
        {
            try
            {
                ClearDb();
            }
            catch (QueueConfigurationException )
            {
                //ignore
            }
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