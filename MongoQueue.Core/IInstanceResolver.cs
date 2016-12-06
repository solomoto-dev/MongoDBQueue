using System;

namespace MongoQueue.Core
{
    public interface IInstanceResolver
    {
        T Get<T>();
        object Get(Type t);
    }
}