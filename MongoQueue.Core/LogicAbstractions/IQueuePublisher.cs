using System.Threading.Tasks;
using MongoQueue.Core.Entities;

namespace MongoQueue.Core.LogicAbstractions
{
    public interface IQueuePublisher
    {
        /// <summary>
        /// Publish message to queue
        /// </summary>
        /// <typeparam name="TMessage">Message payload type and the topic name</typeparam>
        /// <param name="message">Message payload</param>
        /// <param name="ack">write resilence guarantee</param>
        void Publish<TMessage>(TMessage message, Ack ack = Ack.Master);
        /// <summary>
        /// Publish message to queue
        /// </summary>
        /// <param name="topic">topic where message will be published name</param>
        /// <param name="message">Message payload</param>
        /// <param name="ack">write resilence guarantee</param>
        void Publish(string topic, object message, Ack ack = Ack.Master);
        /// <summary>
        /// Publish message to queue
        /// </summary>
        /// <typeparam name="TMessage">Message payload type and the topic name</typeparam>
        /// <param name="message">Message payload</param>
        /// <param name="ack">write resilence guarantee</param>
        Task PublishAsync<TMessage>(TMessage message, Ack ack = Ack.Master);
        /// <summary>
        /// Publish message to queue
        /// </summary>
        /// <param name="topic">topic where message will be published name</param>
        /// <param name="message">Message payload</param>
        /// <param name="ack">write resilence guarantee</param>
        Task PublishAsync(string topic, object message, Ack ack = Ack.Master);
    }
}