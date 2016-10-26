namespace MongoQueue.Core.Common
{
    public class MongoConnection
    {
        public void Initialize(string connectionString, string configuration, string serviceName)
        {
            DatabaseName = (configuration + "-" + serviceName).ToLower();
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; private set; }
        public string DatabaseName { get; private set; }
    }
}