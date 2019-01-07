using System;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Marketplace.Infrastructure
{
    public static class EventDeserializer
    {
        public static object Deserialzie(this ResolvedEvent resolvedEvent)
        {
            var meta = JsonConvert.DeserializeObject<EventMetadata>(
                Encoding.UTF8.GetString(resolvedEvent.Event.Metadata));
            var dataType = Type.GetType(meta.ClrType);
            var jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data);
            var data = JsonConvert.DeserializeObject(jsonData, dataType);
            return data;
        }
    }
}