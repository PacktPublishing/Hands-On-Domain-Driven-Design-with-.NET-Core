using Marketplace.PaidServices.Messages.Ads;
using static Marketplace.EventSourcing.TypeMapper;
using static Marketplace.PaidServices.Messages.Orders.Events;

namespace Marketplace.PaidServices
{
    public static class EventMappings
    {
        public static void MapEventTypes()
        {
            Map<V1.OrderCreated>("OrderCreated");
            Map<V1.ServiceAddedToOrder>("ServiceAddedToOrder");
            Map<V1.ServiceRemovedFromOrder>("ServiceRemovedFromOrder");

            Map<Events.V1.Created>("PaidClassifiedAdCreated");
            Map<Events.V1.ServiceActivated>("AdServiceActivated");
            Map<Events.V1.ServiceDeactivated>("AdServiceDeactivated");
        }
    }
}