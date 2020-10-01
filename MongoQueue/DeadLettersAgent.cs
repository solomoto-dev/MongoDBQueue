using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core.AgentAbstractions;
using MongoQueue.Core.Entities;
using MongoDB.Driver;
using MongoQueue.Core;

namespace MongoQueue
{
    public sealed class DeadLettersAgent : IDeadLettersAgent
    {
        private readonly MongoAgent _agent;

        public DeadLettersAgent(MongoAgent agent)
        {
            _agent = agent;
        }
        public async Task<DeadLetter[]> GetDeadLetters(string route, string topic, CancellationToken cancellationToken)
        {
            var filter = Builders<DeadLetter>.Filter;
            var find = filter.And(filter.Eq(l => l.Topic, topic),
                filter.AnyNin(l => l.ResentTo, new[] { route }));

            var update = Builders<DeadLetter>.Update.Push(x => x.ResentTo, route);
            var collection = _agent.GetCollection<DeadLetter>();
            var cursor = await collection.FindAsync(find, cancellationToken: cancellationToken);
            var deadLetters = await cursor.ToListAsync(cancellationToken);
            var ids = deadLetters.Select(l => l.Id).ToArray();
            await collection.UpdateManyAsync(filter.In(l => l.Id, ids), update, cancellationToken: cancellationToken);
            return deadLetters.ToArray();
        }

        public async Task PublishAsync(string topic, string payload, Ack ack = Ack.Master)
        {
            var collection = _agent
                .GetCollection<DeadLetter>()
                .WithWriteConcern(ack.ToWriteConcern());
            await collection.InsertOneAsync(new DeadLetter
            {
                Id = IdGenerator.GetId(),
                ResentTo = new string[0],
                Payload = payload,
                Topic = topic
            });
        }

        public void Publish(string topic, string payload, Ack ack = Ack.Master)
        {
            var collection = _agent
                .GetCollection<DeadLetter>()
                .WithWriteConcern(ack.ToWriteConcern());

            collection.InsertOne(new DeadLetter
            {
                Id = IdGenerator.GetId(),
                ResentTo = new string[0],
                Payload = payload,
                Topic = topic
            });
        }
    }
}