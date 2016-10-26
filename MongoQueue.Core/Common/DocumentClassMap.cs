using System;
using MongoDB.Bson.Serialization;

namespace MongoQueue.Core.Common
{
    public class DocumentClassMap : BsonClassMap
    {
        public DocumentClassMap(Type classType)
            : base(classType)
        {
        }
    }
}