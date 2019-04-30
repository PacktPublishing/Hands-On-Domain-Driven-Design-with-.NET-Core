using System;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.RavenDb;
using Raven.Client.Documents.Session;
using static Marketplace.PaidServices.Messages.Orders.Events;
using static Marketplace.PaidServices.Projections.ReadModels;

namespace Marketplace.PaidServices.Projections
{
    public static class CompletedOrderProjection
    {
        public static Func<Task> GetHandler(
            IAsyncDocumentSession session,
            object @event
        )
            => @event switch
            {
                V1.OrderFulfilled e =>
                    () => session.Create(e.FromCreated),
                _ => (Func<Task>) null
            };

        static CompletedOrder FromCreated(this V1.OrderFulfilled e)
            => new CompletedOrder
            {
                Id = CompletedOrder.GetDatabaseId(
                    e.OrderId
                ),
                CustomerId = e.CustomerId.ToString(),
                ClassifiedAdId = e.ClassifiedAdId.ToString(),
                Total = e.Total,
                FulfilledAt = e.FulfilledAt,
                OrderedServices = e.Services.Select(
                        s => new CompletedOrder.Service
                        {
                            Description = s.Description,
                            Price = s.Price
                        }
                    )
                    .ToArray()
            };
    }
}