using System;

namespace MongoQueue.Core
{
    public static class IdGenerator
    {
        private static Func<string> Generator;
        public static void SetGenerator(Func<string> generator)
        {
            Generator = generator;
        }

        public static string GetId()
        {
            return Generator();
        }

        public static string Empty => "000000000000000000000000";
    }
}