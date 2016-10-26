using System;

namespace MongoQueueShared.Common
{
    public interface IMessagingLogger
    {
        void Error(Exception exception, string message = null);
        void Info(string message);
        void Debug(string message);
        void Trace(string message);
    }
}