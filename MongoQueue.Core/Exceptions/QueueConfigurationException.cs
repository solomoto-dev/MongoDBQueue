using System;

namespace MongoQueue.Core.Exceptions
{
    public class QueueConfigurationException : Exception
    {
        public QueueConfigurationException() : base("can't access mongoDb")
        {
            
        }
    }
}