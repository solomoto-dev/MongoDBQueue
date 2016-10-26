using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization;

namespace MongoQueueShared.Common
{
    public class DocumentMappingInitializer
    {
        public static void Initialize()
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

        private static int count;
        private static string initType;

        private static void CreateMapping(Type type, HashSet<Type> mappings)
        {
            if (count == 0)
            {
                initType = type.FullName;
            }
            count++;
            if (count > 10)
            {
                //TODO::V:: сменить тип исключения
                //throw new BusinessException(initType);
            }
            mappings.Add(type);
            foreach (var propertyInfo in type.GetProperties().Where(x => !Filter(x.PropertyType)))
            {
                CreateMapping(propertyInfo.PropertyType, mappings);
            }
            count--;
        }

        //TODO::V:: дублирование с CommonMapper
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