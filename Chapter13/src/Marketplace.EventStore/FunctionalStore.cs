using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.EventSourcing;

namespace Marketplace.EventStore
{
    public class FunctionalStore : IFunctionalAggregateStore
    {
        readonly IEventStoreConnection _connection;

        public FunctionalStore(IEventStoreConnection connection)
            => _connection = connection;

        public Task Save<T>(
            long version,
            AggregateState<T>.Result update
        )
            where T : class, IAggregateState<T>, new()
            => _connection.AppendEvents(
                update.State.StreamName, version, update.Events.ToArray()
            );

        public Task<T> Load<T>(Guid id) where T : IAggregateState<T>, new()
            => Load<T>(id, (x, e) => x.When(x, e));

        async Task<T> Load<T>(Guid id, Func<T, object, T> when)
            where T : IAggregateState<T>, new()
        {
            const int maxSliceSize = 4096;

            var state = new T();
            var streamName = state.GetStreamName(id);

            var position = 0L;
            bool endOfStream;
            var events = new List<object>();

            do
            {
                var slice = await _connection
                    .ReadStreamEventsForwardAsync(
                        streamName, position, maxSliceSize, false
                    );
                position = slice.NextEventNumber;
                endOfStream = slice.IsEndOfStream;

                events.AddRange(
                    slice.Events.Select(x => x.Deserialze())
                );
            } while (!endOfStream);

            return events.Aggregate(state, when);
        }
    }
}