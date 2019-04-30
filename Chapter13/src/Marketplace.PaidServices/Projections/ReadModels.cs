using System;
using System.Collections.Generic;

namespace Marketplace.PaidServices.Projections
{
    public static class ReadModels
    {
        public class OrderDraft
        {
            public string Id { get; set; }
            public string ClassifiedAdId { get; set; }
            public string CustomerId { get; set; }
            public double Total { get; set; }
            public List<Service> Services { get; set; }

            public class Service
            {
                public string Type { get; set; }
                public string Description { get; set; }
                public double Price { get; set; }
            }
            
            public static string GetDatabaseId(Guid id)
                => $"OrderDraft/{id}";
        }

        public class CompletedOrder
        {
            public string Id { get; set; }
            public string ClassifiedAdId { get; set; }
            public string CustomerId { get; set; }
            public double Total { get; set; }
            public DateTimeOffset FulfilledAt { get; set; }
            public Service[] OrderedServices { get; set; }

            public class Service
            {
                public string Description { get; set; }
                public double Price { get; set; }
            }
            
            public static string GetDatabaseId(Guid id)
                => $"CompletedOrder/{id}";
        }

        public class ClassifiedAdOrders
        {
            public string Id { get; set; }
            public string CustomerId { get; set; }
            public List<Order> Orders { get; set; }

            public class Order
            {
                public string OrderId { get; set; }
                public double Total { get; set; }
                public DateTimeOffset FulfilledAt { get; set; }
            }
            
            public static string GetDatabaseId(Guid id)
                => $"ClassifiedAdOrders/{id}";
        }
    }
}