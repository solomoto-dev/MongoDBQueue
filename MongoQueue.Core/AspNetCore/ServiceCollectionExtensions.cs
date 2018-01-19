using Microsoft.Extensions.DependencyInjection;
using MongoQueue.Core.IntegrationDefaults;
using MongoQueue.Core.LogicAbstractions;
using System;

namespace MongoQueue.Core.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMongoQueue<TRegistrator>(this IServiceCollection services, Action<MongoQueueConfig> configure) where TRegistrator : IMessagingDependencyRegistrator, new()
        {
            var queueBuilder = new QueueBuilder().AddRegistrator<TRegistrator>(services);
            var config = new MongoQueueConfig(queueBuilder);
            configure(config);
            queueBuilder.AddResolver().Build();
        }
    }

    public sealed class MongoQueueConfig
    {
        private readonly QueueBuilder _queueBuilder;

        internal MongoQueueConfig(QueueBuilder queueBuilder)
        {
            _queueBuilder = queueBuilder;
        }

        public void SetConnectionString(string connectionString, string databaseName)
        {
            _queueBuilder.AddConfiguration(DefaultMessagingConfiguration.Create(connectionString, databaseName));
        }
        public void AddHandler<THandler, TMessage>() where THandler : class, IHandler<TMessage>
        {
            _queueBuilder.AddHandler<THandler, TMessage>();
        }
    }
}