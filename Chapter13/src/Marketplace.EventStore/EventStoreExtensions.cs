using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.EventSourcing;
using Newtonsoft.Json;

namespace Marketplace.EventStore
{
    public static class EventStoreExtensions
    {
        public static Task AppendEvents(
            this IEventStoreConnection connection,
            string streamName,
            long version,
            params object[] events)
        {
            if (events == null || !events.Any()) return Task.CompletedTask;

            var preparedEvents = events
                .Select(
                    @event =>
                        new EventData(
                            Guid.NewGuid(),
                            TypeMapper.GetTypeName(@event.GetType()),
                            true,
                            Serialize(@event),
                            Serialize(
                                new EventMetadata
                                {
                                    ClrType = @event.GetType().FullName
                                }
                            )
                        )
                )
                .ToArray();

            return connection.AppendToStreamAsync(
                streamName,
                version,
                preparedEvents
            );
        }

        static byte[] Serialize(object data) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
    }
}