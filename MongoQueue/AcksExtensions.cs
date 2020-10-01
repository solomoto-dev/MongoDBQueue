using MongoDB.Driver;
using MongoQueue.Core.Entities;

namespace MongoQueue
{
    internal static class AcksExtensions
    {
        public static WriteConcern ToWriteConcern(this Ack ack)
        {
            return ack switch
            {
                Ack.Majority => WriteConcern.WMajority,
                Ack.Master => WriteConcern.W1,
                Ack.None => WriteConcern.Unacknowledged
            };
        }
    }
}