using System;

namespace LegacyReader
{
    public class DomainMessage
    {
        public string Id { get; }
        public string Name { get; }
        public DateTime SomeDate { get; }

        public DomainMessage(string id, string name, DateTime someDate)
        {
            Id = id;
            Name = name;
            SomeDate = someDate;
        }
    }
}