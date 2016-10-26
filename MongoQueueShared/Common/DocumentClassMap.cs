using System;
using MongoDB.Bson.Serialization;

namespace MongoQueueShared.Common
{

    public class DocumentClassMap : BsonClassMap
    {
        public DocumentClassMap(Type classType)
            : base(classType)
        {
        }
    }
}
