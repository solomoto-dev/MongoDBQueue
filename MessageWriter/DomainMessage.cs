namespace MessageWriter
{
    public class DomainMessage
    {
        public string Id { get; }
        public string Name { get; }

        public DomainMessage(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}