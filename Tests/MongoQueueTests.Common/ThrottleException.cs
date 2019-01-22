using System;
using System.Runtime.Serialization;

namespace MongoQueueTests.Common
{
    [Serializable]
    public class ThrottleException : Exception
    {
        public ThrottleException()
        {            
        }

        protected ThrottleException(SerializationInfo info, StreamingContext context)
        {            
        }
    }
}