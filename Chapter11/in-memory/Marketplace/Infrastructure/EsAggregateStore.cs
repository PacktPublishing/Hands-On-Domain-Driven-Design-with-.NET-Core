using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.Framework;
using Newtonsoft.Json;

namespace Marketplace.Infrastructure
{
    public class EsAggregateStore : IAggregateStore
    {
        private readonly IEventStoreConnection _connection;

        public EsAggregateStore(IEventStoreConnection connection)
        {
            _connection = connection;
        }

        public async Task Save<T, TId>(T aggregate) where T : AggregateRoot<TId>
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            var streamName = GetStreamName<T, TId>(aggregate);
            var changes = aggregate.GetChanges().ToArray();

            await _connection.AppendEvents(streamName, aggregate.Version, changes);

            aggregate.ClearChanges();
        }

        public async Task<T> Load<T, TId>(TId aggregateId)
            where T : AggregateRoot<TId>
        {
            if (aggregateId == null)
                throw new ArgumentNullException(nameof(aggregateId));

            var stream = GetStreamName<T, TId>(aggregateId);
            var aggregate = (T) Activator.CreateInstance(typeof(T), true);

            var page = await _connection.ReadStreamEventsForwardAsync(
                stream, 0, 1024, false);

            aggregate.Load(page.Events.Select(
                resolvedEvent => resolvedEvent.Deserialzie()).ToArray());

            return aggregate;
        }

        public async Task<bool> Exists<T, TId>(TId aggregateId)
        {
            var stream = GetStreamName<T, TId>(aggregateId);
            var result = await _connection.ReadEventAsync(stream, 1, false);
            return result.Status != EventReadStatus.NoStream;
        }

        private static byte[] Serialize(object data)
            => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

        private static string GetStreamName<T, TId>(TId aggregateId)
            => $"{typeof(T).Name}-{aggregateId.ToString()}";

        private static string GetStreamName<T, TId>(T aggregate)
            where T : AggregateRoot<TId>
            => $"{typeof(T).Name}-{aggregate.Id.ToString()}";
    }
}