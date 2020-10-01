using Autofac;
using Autofac.Extensions.DependencyInjection;
using MongoQueue.Core;

namespace MongoQueue.Autofac
{
    public static class QueueBuilderExtentions
    {
        public static QueueBuilder AddAutofac(this QueueBuilder queue, ContainerBuilder builder)
        {
            builder.Populate(queue.Registrator);
            return queue;
        }
    }
}