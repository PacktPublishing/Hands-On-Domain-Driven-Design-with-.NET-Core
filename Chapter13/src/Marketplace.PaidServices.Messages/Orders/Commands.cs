using System;

namespace Marketplace.PaidServices.Messages.Orders
{
    public static class Commands
    {
        public static class V1
        {
            public class CreateOrder
            {
                public Guid OrderId { get; set; }
                public Guid ClassifiedAdId { get; set; }
                public Guid CustomerId { get; set; }
            }

            public class AddService
            {
                public Guid OrderId { get; set; }
                public string ServiceType { get; set; }
            }

            public class RemoveService
            {
                public Guid OrderId { get; set; }
                public string ServiceType { get; set; }
            }

            public class FulfillOrder
            {
                public Guid OrderId { get; set; }
            }
        }
    }
}