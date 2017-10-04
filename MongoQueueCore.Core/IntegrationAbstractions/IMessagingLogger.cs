using System;

namespace MongoQueue.Core.IntegrationAbstractions
{
    public interface IMessagingLogger
    {
        void Error(Exception exception, string message = null);
        void Info(string message);
        void Debug(string message);
        void Trace(string message);
    }
}