using System;
using MongoDB.Bson.Serialization;

namespace MongoQueue
{
    public class DocumentClassMap : BsonClassMap
    {
        public DocumentClassMap(Type classType)
            : base(classType)
        {
        }
    }
}