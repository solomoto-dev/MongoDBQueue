namespace SecondReader
{
    public class DomainMessage
    {
        public string Id { get; }
        public string Name { get;  }
        public string UselessField { get;  }

        public DomainMessage(string id, string name, string uselessField)
        {
            Id = id;
            Name = name;
            UselessField = uselessField;
        }
    }
}