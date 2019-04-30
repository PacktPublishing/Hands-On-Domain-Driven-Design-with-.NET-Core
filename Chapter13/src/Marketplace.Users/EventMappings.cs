using static Marketplace.EventSourcing.TypeMapper;
using static Marketplace.Users.Messages.UserProfile.Events;

namespace Marketplace.Users
{
    public static class EventMappings
    {
        public static void MapEventTypes()
        {
            Map<V1.UserRegistered>("UserRegistered");
            Map<V1.UserFullNameUpdated>("UserFullNameUpdated");
        }
    }
}