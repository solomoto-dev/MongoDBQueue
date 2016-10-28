using System.Collections.Generic;
using System.Linq;

namespace MongoQueueTests
{
    public static class ResultHolder
    {
        public static readonly Dictionary<string, string> Results = new Dictionary<string, string>();
        public static int Count => Results.Count;

        public static void Add(string key, string result)
        {
            Results.Add(key, result);
        }

        public static string Get(string key)
        {
            return Results[key];
        }

        public static bool Contains(params string[] keys)
        {
            return keys.All(k => Results.ContainsKey(k));
        }

        public static void Clear()
        {
            Results.Clear();
        }
    }
}