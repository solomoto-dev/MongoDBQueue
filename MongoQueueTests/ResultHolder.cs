using System.Collections.Generic;
using System.Linq;

namespace MongoQueueTests
{
    public class Result
    {
        public string Id { get; }
        public string Name { get; }

        public Result(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public static class ResultHolder
    {
        private static readonly List<Result> Results = new List<Result>();
        public static int Count => Results.Count;
        public static void Add(string key, string result)
        {
            Results.Add(new Result(key, result));
        }

        public static string Get(string key)
        {
            return Results.Where(x => x.Id == key).Select(x => x.Name).FirstOrDefault();
        }

        public static bool Contains(params string[] keys)
        {
            return keys.All(k => Results.Any(x => x.Id == k));
        }

        public static void Clear()
        {
            Results.Clear();
        }
    }
}