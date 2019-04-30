using System;

namespace Marketplace.PaidServices.Messages.Ads
{
    public static class Events
    {
        public class V1
        {
            public class Created
            {
                public Guid ClassifiedAdId { get; set; }
                public Guid SellerId { get; set; }
            }

            public class ServiceActivated
            {
                public Guid ClassifiedAdId { get; set; }
                public string ServiceType { get; set; }
                public DateTimeOffset ActiveUntil { get; set; }
            }

            public class ServiceDeactivated
            {
                public Guid ClassifiedAdId { get; set; }
                public string ServiceType { get; set; }
            }
        }
    }
}