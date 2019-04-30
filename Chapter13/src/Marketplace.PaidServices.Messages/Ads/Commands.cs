using System;

namespace Marketplace.PaidServices.Messages.Ads
{
    public static class Commands
    {
        public static class V1
        {
            public class Create
            {
                public Guid ClassifiedAdId { get; set; }
                public Guid SellerId { get; set; }
            }

            public class FulfillOrder
            {
                public Guid ClassifiedAdId { get; set; }
                public DateTimeOffset When { get; set; }
                public string[] ServiceTypes { get; set; }
            }
        }
    }
}