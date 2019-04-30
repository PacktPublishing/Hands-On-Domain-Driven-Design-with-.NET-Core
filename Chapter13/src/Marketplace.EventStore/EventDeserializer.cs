using System.Text;
using EventStore.ClientAPI;
using Marketplace.EventSourcing;
using Newtonsoft.Json;

namespace Marketplace.EventStore
{
    public static class EventDeserializer
    {
        public static object Deserialze(this ResolvedEvent resolvedEvent)
        {
            var dataType = TypeMapper.GetType(resolvedEvent.Event.EventType);
            var jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data);
            var data = JsonConvert.DeserializeObject(jsonData, dataType);
            return data;
        }

        public static T Deserialze<T>(this ResolvedEvent resolvedEvent)
        {
            var jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
}