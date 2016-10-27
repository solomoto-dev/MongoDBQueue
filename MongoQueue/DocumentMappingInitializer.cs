using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization;
using MongoQueue.Core;
using MongoQueue.Core.AgentAbstractions;
using MongoQueue.Core.Entities;

namespace MongoQueue
{
    public class DocumentMappingInitializer : IDocumentMappingInitializer
    {
        public void Initialize()
        {
            var mappings = new HashSet<Type>();

            CreateMapping(typeof(Envelope), mappings);
            CreateMapping(typeof(Subscriber), mappings);

            foreach (var type in mappings)
            {
                var map = new DocumentClassMap(type);
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                BsonClassMap.RegisterClassMap(map);
            }
        }

        private static void CreateMapping(Type type, HashSet<Type> mappings)
        {
            mappings.Add(type);
            foreach (var propertyInfo in type.GetProperties().Where(x => !Filter(x.PropertyType)))
            {
                CreateMapping(propertyInfo.PropertyType, mappings);
            }
        }

        static bool Filter(Type type)
        {
            return type.IsPrimitive || NoPrimitiveTypes.Contains(type.Name) ||
                   (type.Namespace != null && type.Namespace.Contains("MongoDB"));
        }

        static readonly HashSet<string> NoPrimitiveTypes = new HashSet<string>()
        {
            "String",
            "DateTime",
            "Decimal",
            "ObjectId",
            "TimeSpan"
        };
    }
}