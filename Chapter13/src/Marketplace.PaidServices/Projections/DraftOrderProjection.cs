using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.RavenDb;
using Raven.Client.Documents.Session;
using static Marketplace.PaidServices.Messages.Orders.Events;
using static Marketplace.PaidServices.Projections.ReadModels;

namespace Marketplace.PaidServices.Projections
{
    public static class DraftOrderProjection
    {
        public static Func<Task> GetHandler(
            IAsyncDocumentSession session,
            object @event
        )
        {
            return @event switch
            {
                V1.OrderCreated e =>
                    () => session.Create<OrderDraft>(
                        x =>
                        {
                            x.Id = DbId(e.OrderId);
                            x.CustomerId = e.CustomerId.ToString();
                            x.ClassifiedAdId = e.ClassifiedAdId.ToString();
                            x.Services = new List<OrderDraft.Service>();
                        }
                    ),
                V1.ServiceAddedToOrder e =>
                    () => session.Update<OrderDraft>(
                        DbId(e.OrderId),
                        x => x.Services.Add(
                            new OrderDraft.Service
                            {
                                Type = e.ServiceType,
                                Description = e.Description,
                                Price = e.Price
                            }
                        )
                    ),
                V1.ServiceRemovedFromOrder e =>
                    () => session.Update<OrderDraft>(
                        DbId(e.OrderId),
                        x => x.Services.RemoveAll(s => s.Type == e.ServiceType)
                    ),
                V1.OrderFulfilled e =>
                    () => session.Del(DbId(e.OrderId)),
                V1.OrderDeleted e =>
                    () => session.Del(DbId(e.OrderId)),
                _ => (Func<Task>) null
            };

            string DbId(Guid guid) => OrderDraft.GetDatabaseId(guid);
        }
    }
}