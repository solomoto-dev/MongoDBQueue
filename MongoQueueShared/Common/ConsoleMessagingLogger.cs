using System;

namespace MongoQueueShared.Common
{
    public class ConsoleMessagingLogger : IMessagingLogger
    {
        public void Error(Exception exception, string message = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (message != null)
            {
                Console.WriteLine(message);
            }
            Console.WriteLine(exception.ToString());
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Trace(string message)
        {
            Console.WriteLine(message);
        }
    }
}